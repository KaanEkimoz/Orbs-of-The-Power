namespace com.game
{
    public static class Constants
    {
        public static class Gameplay
        {
            /// <summary>
            /// This is the maximum count of orbs can be held by the player.
            /// </summary>
            // Set a value high enough to avoid exceeding of this count.
            // Set a value low enough to keep any computation related to this constant performent.
            public const int MAX_ORBS_CAN_BE_HELD = 12;

            /// <summary>
            /// This is the maximum count of items player can hold in their inventory at the same time.
            /// </summary>
            public const int MAX_ITEMS_ON_PLAYER = 6;

            public const int DEFAULT_WAVE_COUNT = 5;

            public const float DEFAULT_FORCED_WAVE_DURATION = 120;
        }

        public static class Paranoia
        {
            public const int PARANOIA_SEGMENT_COUNT = 5;
            public const int PARANOIA_VIRTUAL_ENEMY_START_SEGMENT = 1;
            public const int PARANOIA_FAKE_ENEMY_START_SEGMENT = 1;
        }

        public static class Shopping
        {
            public const int PLAYER_SHOP_CAPACITY = 4;
            public const int ORB_SHOP_CAPACITY = 3;
        }

        public static class AssetManagement
        {
            public const string RELEASE_BUILD_ASSET_LABEL = "release";
        }

        public static class Values
        {
            public const int EDGE_VALUE_AMOUNT = 100;
        }
    }
}
