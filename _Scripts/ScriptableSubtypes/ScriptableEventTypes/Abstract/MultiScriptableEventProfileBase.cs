using com.absence.attributes;
using com.game.scriptableeventsystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.scriptables.events
{
    public abstract class MultiScriptableEventProfileBase : ScriptableEventProfileBase
    {
        [Space, Header3("Multi-Event Properties")]
        [SerializeField, DisableIf(nameof(IsSubAsset)), Inline(newButtonId = 2301, delButtonId = 2302)]
        protected List<ScriptableEventProfileBase> m_subEvents = new();
    }
}
