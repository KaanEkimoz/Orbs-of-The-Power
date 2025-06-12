namespace com.game.player
{
    public interface IParanoiaTarget
    {
        public void OnFetchParanoiaAffectionValue(float value);
        public void OnParanoiaSegmentChange(int segmentIndex);
    }
}
