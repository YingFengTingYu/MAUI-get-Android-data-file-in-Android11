using AndroidX.DocumentFile.Provider;

namespace MAUIPickFile
{
    public static class FileHelper
    {
        /// <summary>
        /// Get the file. If the path is folder or the file is not exist, return null.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<SimpleFile> GetFile(string path) => await _getFile(path);

        /// <summary>
        /// Create the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<SimpleFile> CreateFile(string path) => await _createFile(path);

        /// <summary>
        /// Delete the file and return true. If the path is folder, return false.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteFile(string path) => await _deleteFile(path);

        /// <summary>
        /// If file exist return 1. If folder exist return 2. If file and folder do not exist return 0.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<int> FileExist(string path) => await _fileExist(path);

        static readonly string PackageName = AppInfo.PackageName;
        static readonly string AndroidDataPath = "/sdcard/Android/data";
        static readonly string AndroidObbPath = "/sdcard/Android/obb";
        static readonly string SdcardPath1 = "/sdcard";
        static readonly string SdcardPath2 = "/storage/emulated/0";
        static readonly string PathSeparator = "/";

        static string _formatPath(string path)
        {
            if (path.StartsWith(SdcardPath2)) path = SdcardPath1 + path[SdcardPath2.Length..];
            if (path.EndsWith(PathSeparator)) path = path[..^PathSeparator.Length];
            return path;
        }

        static int _checkIfSAF(string path, out string subpath)
        {
            int type = 0;
            path = _formatPath(path);
            if (path.Length > AndroidDataPath.Length && path.StartsWith(AndroidDataPath))
            {
                path = path[(AndroidDataPath.Length + 1)..];
                type = 1;
            }
            else if (path.Length > AndroidObbPath.Length && path.StartsWith(AndroidObbPath))
            {
                path = path[(AndroidObbPath.Length + 1)..];
                type = 2;
            }
            else
            {
                subpath = null;
                return 0;
            }
            if (path.StartsWith(PackageName) && (path.Length == PackageName.Length || path[PackageName.Length] == '/'))
            {
                subpath = null;
                return 0;
            }
            subpath = path;
            return type;
        }

        static bool data = true;
        static bool obb = false;

        static async Task<bool> RequestAndroidDataPermission() => data || (data = await SAFHelper.RequestAndroidDataPermission());

        static async Task<bool> RequestAndroidObbPermission() => obb || (data = await SAFHelper.RequestAndroidObbPermission());

        static async Task<SimpleFile> _getFile(string path)
        {
            int type = _checkIfSAF(path, out string subPath);
            if (type == 1)
            {
                //data
                bool ask = await RequestAndroidDataPermission();
                if (!ask) return null;
                DocumentFile file = SAFHelper.GetSubDocumentFileFromAndroidData(subPath.Split('/'));
                if (file == null || file.IsDirectory) return null;
                return new SimpleFile(path, file);
            }
            else if (type == 2)
            {
                //obb
                bool ask = await RequestAndroidObbPermission();
                if (!ask) return null;
                DocumentFile file = SAFHelper.GetSubDocumentFileFromAndroidObb(subPath.Split('/'));
                if (file == null || file.IsDirectory) return null;
                return new SimpleFile(path, file);
            }
            else
            {
                if (!File.Exists(path)) return null;
                return new SimpleFile(path);
            }
        }

        static async Task<SimpleFile> _createFile(string path)
        {
            int type = _checkIfSAF(path, out string subPath);
            if (type == 1)
            {
                //data
                bool ask = await RequestAndroidDataPermission();
                if (!ask) return null;
                DocumentFile file = SAFHelper.CreateSubDocumentFileFromAndroidData(subPath.Split('/'));
                if (file == null || file.IsDirectory) return null;
                return new SimpleFile(path, file);
            }
            else if (type == 2)
            {
                //obb
                bool ask = await RequestAndroidObbPermission();
                if (!ask) return null;
                DocumentFile file = SAFHelper.CreateSubDocumentFileFromAndroidObb(subPath.Split('/'));
                if (file == null || file.IsDirectory) return null;
                return new SimpleFile(path, file);
            }
            else
            {
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.Create(path);
                }
                return new SimpleFile(path);
            }
        }

        /// <summary>
        /// If file exist return 1. If folder exist return 2. If file and folder do not exist return 0.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static async Task<int> _fileExist(string path)
        {
            int type = _checkIfSAF(path, out string subPath);
            if (type == 1)
            {
                //data
                bool ask = await RequestAndroidDataPermission();
                if (!ask) return 0;
                DocumentFile file = SAFHelper.GetSubDocumentFileFromAndroidData(subPath.Split('/'));
                if (file == null) return 0;
                return file.IsDirectory ? 2 : 1;
            }
            else if (type == 2)
            {
                //obb
                bool ask = await RequestAndroidObbPermission();
                if (!ask) return 0;
                DocumentFile file = SAFHelper.GetSubDocumentFileFromAndroidObb(subPath.Split('/'));
                if (file == null) return 0;
                return file.IsDirectory ? 2 : 1;
            }
            else
            {
                return File.Exists(path) ? 1 : (Directory.Exists(path) ? 2 : 0);
            }
        }

        static async Task<bool> _deleteFile(string path)
        {
            SimpleFile file = await _getFile(path);
            if (file == null) return false;
            file.Delete();
            return true;
        }

        static FileHelper()
        {
            
        }
    }
}
