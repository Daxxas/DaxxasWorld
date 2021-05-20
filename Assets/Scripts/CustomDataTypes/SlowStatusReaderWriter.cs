using Mirror;

public static class SlowStatusReaderWriter
{
    public static void WriteSlowStatus(this NetworkWriter writer, SlowStatus slowStatus)
    {
        writer.WriteSingle(slowStatus.GetSlowAmount());
    }

    public static SlowStatus ReadSlowStatus(this NetworkReader reader)
    {
        return new SlowStatus(reader.ReadSingle());
    }
}
