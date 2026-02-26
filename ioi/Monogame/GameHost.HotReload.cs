namespace ioi
{
    internal partial class GameHost
    {
#if DEBUG
        private bool _isResourcesNeedsHotReload = false;
        private string _changedHotReloadFile;
        private FileSystemWatcher _hotReloadWatcher;

        private void EnableHotReload()
        {
            var resourcesPaths = Path.Combine(Game.Settings.PathProject, "Resources\\BaseGame");

            _hotReloadWatcher = new FileSystemWatcher(resourcesPaths, "*.json")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes |
                   NotifyFilters.CreationTime |
                   NotifyFilters.FileName |
                   NotifyFilters.LastAccess |
                   NotifyFilters.LastWrite |
                   NotifyFilters.Size |
                   NotifyFilters.Security,
                EnableRaisingEvents = true
            };
            _hotReloadWatcher.Filters.Add(".json");
            _hotReloadWatcher.Filters.Add(".tmx");
            _hotReloadWatcher.Filters.Add(".fx");
            _hotReloadWatcher.Filters.Add(".jpg");
            _hotReloadWatcher.Filters.Add(".jpeg");

            _hotReloadWatcher.Changed += (s, e) =>
            {
                _isResourcesNeedsHotReload = true;
                _changedHotReloadFile = e.FullPath;
            };
        }

        private void UpdateResources()
        {
            if (Game.Settings.IsResourceHotReload)
            {
                if (_isResourcesNeedsHotReload)
                {
                    if (!_changedHotReloadFile.Contains("Data\\Scripts"))
                        Content.HotReload(_changedHotReloadFile);

                    _isResourcesNeedsHotReload = false;
                }
            }
        }
    }
#endif
}