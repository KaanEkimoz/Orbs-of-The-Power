using com.absence.attributes;
using com.absence.timersystem;
using com.absence.utilities;
using com.game.ai;
using com.game.altarsystem;
using com.game.enemysystem;
using com.game.events;
using com.game.miscs;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.shopsystem;
using com.game.ui;
using com.game.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game
{
    [DefaultExecutionOrder(-5)]
    public class SceneManager : Singleton<SceneManager>
    {
        public const FindObjectsInactive INCLUDE_INACTIVES_AUTO_FILL = FindObjectsInactive.Include;

        [Header1("Scene Manager")]

        [Space, Header2("Settings")]

        [SerializeField] private AISelection m_defaultAISelection;
        [SerializeField] private bool m_waveCycleEnabled = false;
        [SerializeField] private bool m_lightSystemEnabled = true;

        [Space, Header2("Values")]
        [SerializeField, Min(0f)] private float m_forcedWaveDuration = Constants.Gameplay.DEFAULT_FORCED_WAVE_DURATION;
        [SerializeField, Min(0)] private int m_waveCount = Constants.Gameplay.DEFAULT_WAVE_COUNT;

        [Space, Header2("Fields")]

        [SerializeField, ShowIf(nameof(m_waveCycleEnabled)), Required] OrbShopUI m_orbShopUI;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled)), Required] PlayerShopUI m_playerShopUI;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled)), Required] OrbContainerUI m_orbContainerUI;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled))] OrbUIUpdater m_orbUIUpdater;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled))] OrbUIUpdater m_orbUIUpdater2;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled))] private Transform m_altar;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled))] private SafeZoneController m_safeZone;
        [SerializeField] GameObject m_defaultEnvironmentLight;
        [SerializeField] GameObject m_lightSystemEnvironmentLight;

        public int LevelsGainedCurrentWave => m_levelsGained;
        public int EndedWaveCount => m_wavesEnded;
        public float StartTimeOfCurrentWave => m_waveStartTime;
        public AISelection DefaultAISelection => m_defaultAISelection;

        public bool WaveCycleEnabled => m_waveCycleEnabled;
        public bool LightSystemInUse => m_lightSystemEnabled;
        public bool AnyChallengeActive => !(string.IsNullOrWhiteSpace(m_activeChallenge));

        public int EnemiesKilledByPlayerThisWave => m_enemiesKilledByPlayerThisWave;
        public int EnemiesKilledByPlayerThisRun => m_enemiesKilledByPlayerThisRun;

        public Transform AltarTransform => m_altar;

        int m_levelsGained;
        int m_wavesEnded;
        int m_enemiesKilledByPlayerThisWave;
        int m_enemiesKilledByPlayerThisRun;
        float m_waveStartTime;
        string m_activeChallenge;
        Action<bool> m_onChallengeEnds;
        List<OrbItemProfile> m_orbUpgradeCache;
        List<SpawnerBase> m_spawners;

        ITimer m_forcedWaveTimer;

        public ITimer ForcedWaveTimer => m_forcedWaveTimer;

        public OrbContainerUI OrbContainerUI => m_orbContainerUI;

        protected override void Awake()
        {
            base.Awake();

            if (GameManager.Instance == null)
            {
                Debug.LogError("A SceneManager needs a GameManager to work!");
                enabled = false;
                return;
            }

            Initialize();
            SubscribeToEvents();
            GameManager.Instance.SetState(GameState.NotStarted, true); // starts the game.
        }

        void Initialize()
        {
            if (m_defaultEnvironmentLight != null) 
                m_defaultEnvironmentLight.SetActive(!m_lightSystemEnabled);

            if (m_lightSystemEnvironmentLight != null) 
                m_lightSystemEnvironmentLight.SetActive(m_lightSystemEnabled);
        }

        void SubscribeToEvents()
        {
            GameEventChannel.OnGameStateChanged += OnGameStateChanged;
            GameEventChannel.OnWaveEnded += OnWaveEnd;
            GameEventChannel.OnForcedWaveEnded += OnForcedWaveEnd;
            PlayerEventChannel.OnLevelUp += OnPlayerLevelUp;
            PlayerEventChannel.OnEnemyKill += OnPlayerKillEnemy;

            if (!m_waveCycleEnabled)
                return;

            m_safeZone.OnContactWithPlayer += OnPlayerContactSafeZone;
        }

        void SetSpawners(bool active)
        {
            m_spawners.ForEach(spawner => spawner.IsActive = active);
        }

        void ClearSpawners()
        {
            m_spawners.ForEach(spawner => spawner.Clear());
        }

        #region Callbacks

        private void OnGameStateChanged(GameState prevState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Stateless:
                    break;
                case GameState.RunSelection:
                    break;
                case GameState.NotStarted:
                    DoNotStarted();
                    break;
                case GameState.InWave:
                    DoInWave(prevState);
                    break;
                case GameState.BetweenWaves:
                    DoBetweenWaves(prevState);
                    break;
                default:
                    break;
            }
        }

        void OnPlayerLevelUp(PlayerLevelingLogic logic)
        {
            if (GameManager.Instance.State != GameState.InWave)
                return;

            m_levelsGained += logic.LastLevelGain;
        }

        private void OnPlayerKillEnemy()
        {
            m_enemiesKilledByPlayerThisWave++;
            m_enemiesKilledByPlayerThisRun++;
        }

        private void OnPlayerContactSafeZone(bool entered)
        {
            if (entered)
            {
                if (GameManager.Instance.State == GameState.InWave && m_forcedWaveTimer == null)
                    GameManager.Instance.SetState(GameState.BetweenWaves);
            }

            else
            {
                GameManager.Instance.SetState(GameState.InWave);
            }
        }

        void OnWaveEnd()
        {
            GameManager.Instance.SetState(GameState.BetweenWaves);
        }

        private void OnForcedWaveTimerCompleted(TimerCompletionContext context)
        {
            m_forcedWaveTimer = null;

            if (!context.Succeeded)
                return;

            GameEventChannel.CommitForceWaveEnded();
        }

        private void OnForcedWaveEnd()
        {
            m_safeZone.DeactivateWalls();
        }

        #endregion

        #region State: Not Started

        void DoNotStarted()
        {
            m_spawners = FindObjectsByType<SpawnerBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(spawner => spawner.ControlledBySceneManager).ToList();

            m_orbUpgradeCache = new();
            m_levelsGained = 0;
            m_wavesEnded = 0;

            GameManager.Instance.SetState(GameState.BetweenWaves);
        }

        #endregion

        #region State: Between Waves

        void DoBetweenWaves(GameState prevState)
        {
            SetSpawners(false);
            ClearSpawners();

            if (prevState == GameState.NotStarted)
                return;

            m_orbContainerUI.HardRedraw();
            m_playerShopUI.SoftReroll();

            m_wavesEnded++;
            //Game.Pause();

            if (!m_waveCycleEnabled)
            {
                GameManager.Instance.SetState(GameState.InWave);
                return;
            }

            m_orbUpgradeCache = m_orbContainerUI.Container.UpgradeCache;

            //foreach (EnemyCombatant enemy in GameObject.FindObjectsByType<EnemyCombatant>(
            //    FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            //{
            //    if (DropManager.Instance != null) DropManager.Instance.Enabled = false;
            //    enemy.Die(DeathCause.Internal);
            //    if (DropManager.Instance != null) DropManager.Instance.Enabled = true;
            //}

            m_orbShopUI.Hide(true);
            m_orbContainerUI.Hide(true);
            m_playerShopUI.Hide(true);

            m_orbContainerUI.SoftRefresh();

            if (m_levelsGained > 0)
            {
                //EnterLevelUpMenu();
                //return;
            }

            //EnterShop();

            //if (m_orbContainerUI.Container.UpgradeCache != null && m_orbContainerUI.Container.UpgradeCache.Count > 0) 
                //EnterOrbInventoryTemporarily();
        }
        public OrbShopUI EnterOrbUpgradeScreen(IShop<OrbItemProfile> shop, bool reroll = true)
        {
            Game.Pause();

            m_orbShopUI.Show(shop, reroll);
            m_orbShopUI.Title.text = "Pick an Element";
            m_orbShopUI.InventoryButton.OnClick += EnterOrbInventoryTemporarily;
            m_orbShopUI.PassButton.OnClick += PassOrbUpgrades;
            m_orbShopUI.OnItemBought -= OnOrbUpgradeBought;
            m_orbShopUI.OnItemBought += OnOrbUpgradeBought;

            return m_orbShopUI;
        }
        void EnterOrbInventoryTemporarily()
        {
            m_orbContainerUI.Show(false, true);
            m_orbContainerUI.RefreshButtons();
            m_orbContainerUI.BackButton.Visibility = () => true;
            m_orbContainerUI.BackButton.OnClick += () => m_orbContainerUI.Hide(false);
            m_orbContainerUI.ConfirmButton.OnClick += OnConfirmUpgrades;
        }
        public void EnterOrbInventory()
        {
            m_orbContainerUI.Show(false, false);
            m_orbContainerUI.RefreshButtons();
            m_orbContainerUI.BackButton.Visibility = () => false;
            m_orbContainerUI.ConfirmButton.OnClick += OnConfirmUpgrades;
        }
        public void ExitOrbInventory()
        {
            m_orbContainerUI.Hide(false);
        }
        void PassOrbUpgrades()
        {
            m_levelsGained = 0;

            m_orbShopUI.Hide(false);
            //EnterShop();

            if ((m_orbContainerUI.Container.UpgradeCache != null && m_orbContainerUI.Container.UpgradeCache.Count > 0) || 
                (m_orbUpgradeCache != null && m_orbUpgradeCache.Count > 0))
                EnterOrbInventory();
            else
            {
                if (GameManager.Instance.State != GameState.BetweenWaves)
                    Game.Resume();
            }
        }
        void OnOrbUpgradeBought(OrbItemProfile profile)
        {
            if (m_orbUpgradeCache == null) m_orbUpgradeCache = new();
            m_orbUpgradeCache.Add(profile);
            m_orbContainerUI.SetUpgradeCache(m_orbUpgradeCache);
            m_orbContainerUI.SoftRefresh();
            //m_levelsGained--;

            //if (m_levelsGained > 0)
            //{
            //    EnterLevelUpMenu();
            //    return;
            //}

            //EnterShop();

            m_orbShopUI.Hide(false);

            if (m_orbUpgradeCache.Count > 0)
                EnterOrbInventory();
        }
        void OnConfirmUpgrades()
        {
            m_orbUpgradeCache = new(m_orbContainerUI.Container.UpgradeCache);
            m_orbContainerUI.SoftRefresh();
            m_orbContainerUI.Hide(false);

            if (GameManager.Instance.State != GameState.BetweenWaves)
                Game.Resume();

            if (m_orbUIUpdater != null) m_orbUIUpdater.Refresh();
            if (m_orbUIUpdater2 != null) m_orbUIUpdater2.Refresh();
        }
        public void EnterShop()
        {
            Game.Pause();

            m_orbShopUI.Hide(false);
            m_playerShopUI.Show(false);
            m_playerShopUI.InventoryButton.OnClick += EnterOrbInventoryTemporarily;
            m_playerShopUI.ProceedButton.OnClick += ExitShop;

            if (m_orbContainerUI.Container.UpgradeCache != null && m_orbContainerUI.Container.UpgradeCache.Count > 0)
                EnterOrbInventoryTemporarily();
        }
        void ExitShop()
        {
            m_playerShopUI.Hide(false);
            Game.Resume();
            //GameManager.Instance.SetState(GameState.InWave);
        }

        #endregion

        #region State: In Wave

        void DoInWave(GameState prevState)
        {
            m_forcedWaveTimer?.Fail();

            Game.Resume();
            m_waveStartTime = Time.time;
            m_enemiesKilledByPlayerThisWave = 0;

            if (!m_waveCycleEnabled)
                return;

            m_forcedWaveTimer = Timer.Create(m_forcedWaveDuration)
                .OnComplete(OnForcedWaveTimerCompleted);

            m_safeZone.ActivateWalls();

            SetSpawners(true);

            //if (prevState == GameState.BetweenWaves)
            //m_playerShopUI.Hide(true);

            m_playerShopUI.Hide(true);

            if (m_orbUIUpdater != null) m_orbUIUpdater.Refresh();
            if (m_orbUIUpdater2 != null) m_orbUIUpdater2.Refresh();
        }

        #endregion

        public bool StartChallenge(string challengeToken, Action<bool> onChallengeEnds)
        {
            if (AnyChallengeActive)
                return false;

            if (string.IsNullOrWhiteSpace(challengeToken))
                return false;

            m_activeChallenge = challengeToken;

            SetSpawners(false);
            ClearSpawners();
            m_forcedWaveTimer?.Pause();

            m_onChallengeEnds = onChallengeEnds;

            return true;
        }

        public void EndChallenge(string challengeToken, bool success)
        {
            if (!AnyChallengeActive) 
                return;

            if (m_activeChallenge != challengeToken)
                return;

            SetSpawners(true);
            ClearSpawners();
            m_forcedWaveTimer?.Resume();
            m_activeChallenge = null;

            m_onChallengeEnds?.Invoke(success);
            m_onChallengeEnds = null;
        }

        [Button("Search and Auto-Fill")]
        void AutoFill()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            UnityEditor.Undo.RecordObject(this, "Scene Manager Auto-Fill (Inspector)");
#endif

            if (m_waveCycleEnabled)
            {
                m_orbShopUI = FindFirstObjectByType<OrbShopUI>(INCLUDE_INACTIVES_AUTO_FILL);
                m_playerShopUI = FindFirstObjectByType<PlayerShopUI>(INCLUDE_INACTIVES_AUTO_FILL);
                m_orbContainerUI = FindFirstObjectByType<OrbContainerUI>(INCLUDE_INACTIVES_AUTO_FILL);

                OrbUIUpdater[] orbUIUpdaters = FindObjectsByType<OrbUIUpdater>(FindObjectsSortMode.None);
                bool hasUIUpdaters = !(orbUIUpdaters == null || orbUIUpdaters.Length == 0);

                if (hasUIUpdaters)
                {
                    m_orbUIUpdater = orbUIUpdaters[0];
                    if (orbUIUpdaters.Length > 1) m_orbUIUpdater2 = orbUIUpdaters[1];
                }

                AltarBehaviour altar = FindFirstObjectByType<AltarBehaviour>();
                m_altar = altar != null ? altar.transform : null;

                SafeZoneController safeZone = FindFirstObjectByType<SafeZoneController>();
                m_safeZone = safeZone != null ? safeZone : null;
            }

#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        [Button("Toggle Light System")]
        void ToggleLights()
        {
            if (Application.isPlaying)
                return;

#if UNITY_EDITOR
            UnityEditor.Undo.IncrementCurrentGroup();
            UnityEditor.Undo.SetCurrentGroupName("Scene Manager Toggle Lights (Inspector)");
            int undoGroupIndex = UnityEditor.Undo.GetCurrentGroup();

            UnityEditor.Undo.RecordObject(this, "0");
#endif

            m_lightSystemEnabled = !m_lightSystemEnabled;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif

            if (m_lightSystemEnvironmentLight != null)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(m_lightSystemEnvironmentLight, "1");
#endif

                m_lightSystemEnvironmentLight.SetActive(m_lightSystemEnabled);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_lightSystemEnvironmentLight);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(m_lightSystemEnvironmentLight);
#endif
            }

            if (m_defaultEnvironmentLight != null)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(m_defaultEnvironmentLight, "2");
#endif

                m_defaultEnvironmentLight.SetActive(!m_lightSystemEnabled);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_defaultEnvironmentLight);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(m_defaultEnvironmentLight);
#endif
            }

#if UNITY_EDITOR
            UnityEditor.Undo.CollapseUndoOperations(undoGroupIndex);
#endif
        }
    }
}
