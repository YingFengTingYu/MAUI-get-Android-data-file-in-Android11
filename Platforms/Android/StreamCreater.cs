namespace MAUIPickFile
{
    public static class StreamCreater
    {
        /// <summary>
        /// Buffer size when read or write the java file stream.
        /// </summary>
        public static int BufferSize = 81920;

        //use MemoryStream to store java stream

        /// <summary>
        /// Get the MemoryStream of file. (If you write this stream, the file will not be changed. Use StreamCreater.Save() to save!)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<Stream> Open(string filePath)
        {
            //Get the java stream
            SimpleFile simpleFile = await FileHelper.GetFile(filePath);
            if (simpleFile?.Document?.Length() == 0) return new MemoryStream();
            using (Java.IO.InputStream stream = simpleFile.CreateInputStream())
            {
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[BufferSize];
                int number = BufferSize;
                while (number == BufferSize)
                {
                    number = stream.Read(buffer, 0, BufferSize);
                    ms.Write(buffer, 0, number);
                }
                ms.Position = 0;
                return ms;
            }
        }

        public static async Task<Stream> OpenOrCreate(string filePath)
        {
            //Get the java stream
            SimpleFile simpleFile = await FileHelper.CreateFile(filePath);
            if (simpleFile?.Document?.Length() == 0) return new MemoryStream();
            using (Java.IO.InputStream stream = simpleFile.CreateInputStream())
            {
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[BufferSize];
                int number = BufferSize;
                while (number == BufferSize)
                {
                    number = stream.Read(buffer, 0, BufferSize);
                    ms.Write(buffer, 0, number);
                }
                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// Save the Stream to the file.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task Save(Stream ms, string filePath)
        {
            //Delete the original file
            await FileHelper.DeleteFile(filePath);
            SimpleFile simpleFile = await FileHelper.CreateFile(filePath);
            using (Java.IO.OutputStream stream = simpleFile.CreateOutputStream())
            {
                long lastPosition = ms.Position;
                ms.Position = 0;
                byte[] buffer = new byte[BufferSize];
                int number = BufferSize;
                while (number == BufferSize)
                {
                    number = ms.Read(buffer, 0, BufferSize);
                    stream.Write(buffer, 0, number);
                }
                ms.Position = lastPosition;
            }
        }
    }
}
