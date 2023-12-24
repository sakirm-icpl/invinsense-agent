namespace Tensible.Modules
{
    /// <summary>
    /// Base AnsibleModule class
    /// </summary>
    internal abstract class ModuleBase
    {
        protected ModuleBase(string module)
        {
            ModuleName = module;
        }

        public string ModuleName { get; }
        
        public abstract bool Validate();

        public abstract void Execute();
    }
}
