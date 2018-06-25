using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models
{
    public class MSBuild
    {
        public interface IProject : IElement, IChooseOwner, IImportOwner, IItemGroupOwner, IProjectExtensionsOwner, IUsingTaskOwner, IPropertyGroupOwner
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
        public interface IPropertyGroupOwner : IElement
        {

        }
        /// <summary>
        /// Required root element of an MSBuild project file.
        /// </summary>
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

            /// <summary>
            /// Contains a set of tasks for MSBuild to sequentially execute. Tasks are specified by using the Task element. There may be zero or more Target elements in a project.
            /// </summary>
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
                [Name(null)]
                public class Task : BaseElement<Target>
                {
                    [Attr("Condition", false)]
                    public string Condition { get; set; }
                    /// <summary>
                    /// Optional attribute. Can contain one of the following values:
                    /// <para>- WarnAndContinue or true. When a task fails, subsequent tasks in the Target element and the build continue to execute, and all errors from the task are treated as warnings.</para>
                    /// <para>- ErrorAndContinue.When a task fails, subsequent tasks in the Target element and the build continue to execute, and all errors from the task are treated as errors.</para>
                    /// <para>- ErrorAndStop or false (default). When a task fails, the remaining tasks in the Target element and the build aren't executed, and the entire Target element and the build is considered to have failed.</para>
                    /// Versions of the .NET Framework before 4.5 supported only the true and false values.
                    /// For more information, see https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-ignore-errors-in-tasks
                    /// </summary>
                    [Attr("ContinueOnError", false)]
                    public string ContinueOnError { get; set; }
                    /// <summary>
                    /// Required if the task class contains one or more properties labeled with the [Required] attribute.
                    /// A user-defined task parameter that contains the parameter value as its value.
                    /// There can be any number of parameters in the Task element, with each attribute mapping to a .NET property in the task class.
                    /// </summary>
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
        }
        /// <summary>
        /// A grouping element for individual properties. Properties are specified by using the Property element. There may be zero or more PropertyGroup elements in a project.
        /// </summary>
        [Child(typeof(Property), 0, -1)]
        [Name("PropertyGroup")]
        public class PropertyGroup : BaseElement<IPropertyGroupOwner>
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }

            /// <summary>
            /// A user defined property name, which contains the property value. There may be zero or more Property elements in a PropertyGroup element.
            /// </summary>
            [Name(null)]
            public class Property : BaseStringElement<PropertyGroup, ElementString>
            {
                [Attr("Condition", false)]
                public string Condition { get; set; }
            }
        }
        /// <summary>
        /// Provides a way to register tasks in MSBuild. There may be zero or more UsingTask elements in a project.
        /// </summary>
        [Name("UsingTask")]
        public class UsingTask : BaseElement<IUsingTaskOwner>
        {

        }
        /// <summary>
        /// Provides a way to persist non-MSBuild information in an MSBuild project file. There may be zero or one ProjectExtensions elements in a project.
        /// </summary>
        [Name("ProjectExtensions")]
        public class ProjectExtensions : BaseElement<IProjectExtensionsOwner>
        {

        }
        /// <summary>
        /// A grouping element for individual items. Items are specified by using the Item element. There may be zero or more ItemGroup elements in a project.
        /// </summary>
        [Name("ItemGroup")]
        public class ItemGroup : BaseElement<IItemGroupOwner>
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }

            [Name(null)]
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
        /// <summary>
        /// A grouping element for individual items. Items are specified by using the Item element. There may be zero or more ItemGroup elements in a project.
        /// </summary>
        [Name("ImportGroup")]
        [Child(typeof(Import), 0, -1)]
        public class ImportGroup : BaseElement<Project>, IImportOwner
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }
        /// <summary>
        /// Enables a project file to import another project file. There may be zero or more Import elements in a project.
        /// </summary>
        [Name("Import")]
        public class Import : BaseElement<IImportOwner>
        {
            [Attr("Project", true)]
            public string Project { get; set; }
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }
        /// <summary>
        /// Evaluates child elements to select one set of ItemGroup elements and/or PropertyGroup elements to evaluate.
        /// </summary>
        [Name("Choose")]
        [Child(typeof(Otherwise), 0, -1)]
        [Child(typeof(When), 1, -1)]
        public class Choose : BaseElement<IChooseOwner> { }
        /// <summary>
        /// Specifies the block of code to execute if the conditions of all When elements evaluate to false.
        /// </summary>
        [Name("Otherwise")]
        [Child(typeof(Choose), 0, -1)]
        [Child(typeof(PropertyGroup), 0, -1)]
        [Child(typeof(ItemGroup), 0, -1)]
        public class Otherwise : BaseElement<Choose>, IChooseOwner, IPropertyGroupOwner, IItemGroupOwner { }
        /// <summary>
        /// Specifies a possible block of code for the Choose element to select.
        /// </summary>
        [Name("When")]
        [Child(typeof(Choose), 0, -1)]
        [Child(typeof(PropertyGroup), 0, -1)]
        [Child(typeof(ItemGroup), 0, -1)]
        public class When : BaseElement<Choose>, IChooseOwner, IPropertyGroupOwner, IItemGroupOwner
        {
            [Attr("Condition", true)]
            public string Condition { get; set; }
        }
    }
}
