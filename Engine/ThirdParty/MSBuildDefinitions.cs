using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models
{
    public class MSBuild
    {
        public interface IProject : IElement, IChooseOwner, IImportOwner, IItemGroupOwner, IProjectExtensionsOwner, IUsingTaskOwner
        {

        }
        public interface IChooseOwner : IElement
        {

        }
        public interface IImportOwner : IElement
        {

        }
        public interface IItemGroupOwner : IElement
        {

        }
        public interface IProjectExtensionsOwner : IElement
        {

        }
        public interface IUsingTaskOwner : IElement
        {

        }

        [Name("Project")]
        [Child(typeof(Choose), 0, -1)]
        [Child(typeof(Import), 0, -1)]
        [Child(typeof(ItemGroup), 0, -1)]
        [Child(typeof(ProjectExtensions), 0, 1)]
        [Child(typeof(PropertyGroup), 0, -1)]
        [Child(typeof(Target), 0, -1)]
        [Child(typeof(UsingTask), 0, -1)]
        public class Project : BaseElement<IElement>, IProject
        {
            public Choose[] ChooseElements => GetChildren<Choose>();
            public Import[] ImportElements => GetChildren<Import>();
            public ItemGroup[] ItemGroupElements => GetChildren<ItemGroup>();
            public ProjectExtensions[] ProjectExtensionsElements => GetChildren<ProjectExtensions>();
            public PropertyGroup[] PropertyGroupElements => GetChildren<PropertyGroup>();
            public Target[] TargetElements => GetChildren<Target>();
            public UsingTask[] UsingTaskElements => GetChildren<UsingTask>();

            [Attr("DefaultTargets", false)]
            public string DefaultTargets { get; set; }
            [Attr("InitialTargets", false)]
            public string InitialTargets { get; set; }
            [Attr("ToolsVersion", false)]
            public string ToolsVersion { get; set; }
            [Attr("TreatAsLocalProperty", false)]
            public string TreatAsLocalProperty { get; set; }
            [Attr("xmlns", true)]
            [DefaultValue("http://schemas.microsoft.com/developer/msbuild/2003")]
            public string Schema { get; set; }

            [Child(typeof(Task), 0, -1)]
            [Child(typeof(PropertyGroup), 0, -1)]
            [Child(typeof(ItemGroup), 0, -1)]
            [Child(typeof(OnError), 0, -1)] //Must be written last!
            [Name("Target")]
            public class Target : BaseElement<Project>, IItemGroupOwner
            {
                [Attr("Name", true)]
                public string Name { get; set; }
                [Attr("Condition", false)]
                public string Condition { get; set; }
                [Attr("Inputs", false)]
                public string Inputs { get; set; }
                [Attr("Outputs", false)]
                public string Outputs { get; set; }
                [Attr("Returns", false)]
                public string Returns { get; set; }
                [Attr("KeepDuplicateOutputs", false)]
                public string KeepDuplicateOutputs { get; set; }
                [Attr("BeforeTargets", false)]
                public string BeforeTargets { get; set; }
                [Attr("AfterTargets", false)]
                public string AfterTargets { get; set; }
                [Attr("DependsOnTargets", false)]
                public string DependsOnTargets { get; set; }
                [Attr("Label", false)]
                public string Label { get; set; }

                [Name("OnError")]
                public class OnError : BaseElement<Target>
                {
                    [Attr("Condition", false)]
                    public string Condition { get; set; }
                    [Attr("ExecuteTargets", true)]
                    public string ExecuteTargets { get; set; }
                }

                [Child(typeof(Output), 0, -1)]
                [Name("Task")]
                public class Task : BaseElement<Target>
                {
                    [Attr("Condition", false)]
                    public string Condition { get; set; }
                    [Attr("ContinueOnError", false)]
                    public string ContinueOnError { get; set; }
                    [Attr("Parameter", false)]
                    public string Parameter { get; set; }
                    
                    [Name("Output")]
                    public class Output : BaseElement<Task>
                    {
                        [Attr("TaskParameter", false)]
                        public string TaskParameter { get; set; }
                        /// <summary>
                        /// Required but cannot be used it ItemName is used.
                        /// </summary>
                        [Attr("PropertyName", false)]
                        public string PropertyName { get; set; }
                        /// <summary>
                        /// Required but cannot be used it PropertyName is used.
                        /// </summary>
                        [Attr("ItemName", false)]
                        public string ItemName { get; set; }
                        [Attr("Condition", false)]
                        public string Condition { get; set; }
                    }
                }
            }
            
            [Child(typeof(Property), 0, -1)]
            [Name("PropertyGroup")]
            public class PropertyGroup : BaseElement<Project>
            {
                [Attr("Condition", false)]
                public string Condition { get; set; }
                
                [Name(null)]
                public class Property : BaseStringElement<PropertyGroup, ElementString>
                {
                    [Attr("Condition", false)]
                    public string Condition { get; set; }
                }
            }
        }

        [Name("UsingTask")]
        public class UsingTask : BaseElement<IUsingTaskOwner>
        {

        }

        [Name("ProjectExtensions")]
        public class ProjectExtensions : BaseElement<IProjectExtensionsOwner>
        {

        }

        [Name("ItemGroup")]
        public class ItemGroup : BaseElement<IItemGroupOwner>
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }

            [Name("Item")]
            public class Item : BaseElement<ItemGroup>
            {
                [Attr("Include", true)]
                public string Include { get; set; }
                [Attr("Exclude", false)]
                public string Exclude { get; set; }
                [Attr("Condition", false)]
                public string Condition { get; set; }
                [Attr("Remove", false)]
                public string Remove { get; set; }
                [Attr("KeepMetadata", false)]
                public string KeepMetadata { get; set; }
                [Attr("RemoveMetadata", false)]
                public string RemoveMetadata { get; set; }
                [Attr("KeepDuplicates", false)]
                public string KeepDuplicates { get; set; }

                [Name(null)]
                public class ItemMetadata : BaseStringElement<Item, ElementString>
                {
                    [Attr("Condition", false)]
                    public string Condition { get; set; }
                }
            }
        }

        [Name("ImportGroup")]
        [Child(typeof(Import), 0, -1)]
        public class ImportGroup : BaseElement<Project>, IImportOwner
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }

        [Name("Import")]
        public class Import : BaseElement<IImportOwner>
        {
            [Attr("Project", true)]
            public string Project { get; set; }
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }

        [Name("Choose")]
        public class Choose : BaseElement<IChooseOwner>
        {

        }
    }
}
