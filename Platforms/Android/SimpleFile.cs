using AndroidX.DocumentFile.Provider;

namespace MAUIPickFile
{
    public class SimpleFile
    {
        public string Path { get; }
        public DocumentFile Document { get; }
        bool UseDocument { get; }
        public SimpleFile()
        {
        }

        public SimpleFile(string p) : this(p, null)
        {
        }

        public SimpleFile(string p, DocumentFile d)
        {
            Path = p;
            Document = d;
            UseDocument = d != null;
        }

        public void Delete()
        {
            if (UseDocument)
            {
                Document.Delete();
            }
            else
            {
                File.Delete(Path);
            }
        }

        public void Rename(string newName)
        {
            if (UseDocument)
            {
                Document.RenameTo(newName);
            }
            else
            {
                File.Copy(Path, System.IO.Path.GetDirectoryName(Path) + System.IO.Path.PathSeparator + newName);
            }
        }

        public Java.IO.OutputStream CreateOutputStream()
        {
            return UseDocument ? (SAFHelper.Activity.ContentResolver.OpenOutputStream(Document.Uri) as Android.Runtime.OutputStreamInvoker).BaseOutputStream : new Java.IO.FileOutputStream(Path);
        }

        public Java.IO.InputStream CreateInputStream()
        {
            return UseDocument ? (SAFHelper.Activity.ContentResolver.OpenInputStream(Document.Uri) as Android.Runtime.InputStreamInvoker).BaseInputStream : new Java.IO.FileInputStream(Path);
        }
    }
}
