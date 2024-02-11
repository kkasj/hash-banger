public class RangeUpdate {
    public UpdateType UpdateType { get; set; }
    public UpdateSource UpdateSource { get; set; }
    public Range Range { get; set; }

    public RangeUpdate(UpdateType updateType, UpdateSource updateSource, Range range)
    {
        UpdateType = updateType;
        UpdateSource = updateSource;
        Range = range;
    }
}