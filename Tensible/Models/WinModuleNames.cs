namespace Tensible.Models
{
    public struct WinModuleNames
    {
        //Creates, touches or removes files or directories
        public const string WIN_FILE = "ansible.windows.win_file";

        //Copies files to remote locations on windows hosts
        public const string WIN_COPY = "ansible.windows.win_copy";

        //Downloads file from HTTP, HTTPS, or FTP to node
        public const string WIN_GET_URL = "ansible.windows.win_get_url";

        //Manage and query Windows services
        public const string WIN_SERVICE = "ansible.windows.win_service";

        //Installs/uninstalls an installable package
        public const string WIN_PACKAGE = "ansible.windows.win_package";

        //Add, change, or remove registry keys and values
        public const string WIN_REGEDIT = "ansible.windows.win_regedit";

        // Manage Windows User Rights
        public const string WIN_USER_RIGHT = "ansible.windows.win_user_right";
    }
}
