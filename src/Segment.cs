namespace LEDBridge
{
    public class Segment
    {
        Output mOutput;
        Group mGroup;

        public Segment(Output output, Group group)
        {
            mOutput = output;
            mGroup = group;
        }
    }
}