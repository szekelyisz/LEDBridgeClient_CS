namespace LEDBridge
{
    public class Group
    {
        public readonly uint id;
        private string mName;
        public string Name => mName;
        private uint mLength;
        public uint Length => mLength;
        private uint mCpp;
        public uint Cpp => mCpp;

        public Group(uint id, string name, uint length, uint cpp)
        {
            this.id = id;
            mName = name;
            mLength = length;
            mCpp = cpp;
        }

        public void Update(string name, uint length, uint cpp)
        {
            mName = name;
            mLength = length;
            mCpp = cpp;
        }
    }
}