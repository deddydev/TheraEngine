using System.ComponentModel;
using TheraEngine.Core.Files.XML;

namespace TheraEngine.ThirdParty
{
    public class MSBuild
    {
        public interface IItemOwner : IElement { }
        public interface IChooseOwner : IElement { }
        public interface IImportOwner : IElement { }
        public interface IItemGroupOwner : IElement { }
        public interface IProjectExtensionsOwner : IElement { }
        public interface IUsingTaskOwner : IElement { }
        public interface IPropertyGroupOwner : IElement { }
        public interface IProject : IElement, IChooseOwner, IImportOwner, IItemGroupOwner, IProjectExtensionsOwner, IUsingTaskOwner, IPropertyGroupOwner, IRoot { }

        /// <summary>
        /// Required root element of an MSBuild project file.
        /// </summary>
        [ElementName("Project")]
        [ElementChild(typeof(Choose), 0, 1)]
        [ElementChild(typeof(Import), 0, -1)]
        [ElementChild(typeof(ItemDefinitionGroup), 0, -1)]
        [ElementChild(typeof(ItemGroup), 0, -1)]
        [ElementChild(typeof(ProjectExtensions), 0, 1)]
        [ElementChild(typeof(PropertyGroup), 0, -1)]
        [ElementChild(typeof(Sdk), 0, 1)]
        [ElementChild(typeof(Target), 0, -1)]
        [ElementChild(typeof(UsingTask), 0, -1)]
        public class Project : BaseElement<IElement>, IProject
        {
            [Browsable(false)]
            public Choose ChooseElement => GetChild<Choose>();
            [Browsable(false)]
            public Import[] ImportElements => GetChildren<Import>();
            [Browsable(false)]
            public ItemDefinitionGroup[] ItemDefinitionGroupElements => GetChildren<ItemDefinitionGroup>();
            [Browsable(false)]
            public ItemGroup[] ItemGroupElements => GetChildren<ItemGroup>();
            [Browsable(false)]
            public ProjectExtensions ProjectExtensionsElement => GetChild<ProjectExtensions>();
            [Browsable(false)]
            public PropertyGroup[] PropertyGroupElements => GetChildren<PropertyGroup>();
            [Browsable(false)]
            public Sdk SdkElement => GetChild<Sdk>();
            [Browsable(false)]
            public Target[] TargetElements => GetChildren<Target>();
            [Browsable(false)]
            public UsingTask[] UsingTaskElements => GetChildren<UsingTask>();

            /// <summary>
            /// Optional attribute.
            /// The default target or targets to be the entry point of the build if no target has been specified. 
            /// Multiple targets are semi-colon(;) delimited.
            /// If no default target is specified in either the DefaultTargets attribute or the MSBuild command line, the engine executes the first target in the project file after the Import elements have been evaluated.
            /// </summary>
            [Attr("DefaultTargets", false)]
            public string DefaultTargets { get; set; }
            /// <summary>
            /// Optional attribute.
            /// The initial target or targets to be run before the targets specified in the DefaultTargets attribute or on the command line.
            /// Multiple targets are semi-colon (;) delimited.
            /// </summary>
            [Attr("InitialTargets", false)]
            public string InitialTargets { get; set; }
            /// <summary>
            /// Optional attribute. 
            /// The SDK name and optional version to use to create implicit Import statements that are added to the .proj file.
            /// If no version is specified, MSBuild will attempt to resolve a default version.
            /// For example, &lt;Project Sdk = "Microsoft.NET.Sdk" /&gt; or &lt;Project Sdk= "My.Custom.Sdk/1.0.0" /&gt;.
            /// </summary>
            [Attr("Sdk", false)]
            public string Sdk { get; set; }
            /// <summary>
            /// Optional attribute.
            /// The version of the toolset MSBuild uses to determine the values for $(MSBuildBinPath) and $(MSBuildToolsPath).
            /// </summary>
            [Attr("ToolsVersion", false)]
            public string ToolsVersion { get; set; }
            /// <summary>
            /// Optional attribute.
            /// <para>
            /// Property names that won't be considered to be global.
            /// This attribute prevents specific command-line properties from overriding property values that are set in a project or targets file and all subsequent imports. 
            /// Multiple properties are semi-colon (;) delimited.
            /// </para><para>
            /// Normally, global properties override property values that are set in the project or targets file.
            /// If the property is listed in the TreatAsLocalProperty value, the global property value doesn't override property values that are set in that file and any subsequent imports. 
            /// For more information, see https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-build-the-same-source-files-with-different-options.
            /// </para><para>
            /// Note: You set global properties at a command prompt by using the /property (or /p) switch. 
            /// You can also set or modify global properties for child projects in a multi-project build by using the Properties attribute of the MSBuild task. 
            /// For more information, see https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-task.
            /// </para>
            /// </summary>
            [Attr("TreatAsLocalProperty", false)]
            public string TreatAsLocalProperty { get; set; }
            /// <summary>
            /// Optional attribute.
            /// When specified, the xmlns attribute must have the value of "http://schemas.microsoft.com/developer/msbuild/2003".
            /// </summary>
            [Attr("xmlns", false)]
            [DefaultValue("http://schemas.microsoft.com/developer/msbuild/2003")]
            public string Schema { get; set; }

            /// <summary>
            /// Contains a set of tasks for MSBuild to sequentially execute. Tasks are specified by using the Task element. There may be zero or more Target elements in a project.
            /// </summary>
            [ElementChild(typeof(Task), 0, -1)]
            [ElementChild(typeof(PropertyGroup), 0, -1)]
            [ElementChild(typeof(ItemGroup), 0, -1)]
            [ElementChild(typeof(OnError), 0, -1)] //Must be written last!
            [ElementName("Target")]
            public class Target : BaseElement<Project>, IItemGroupOwner
            {
                [Browsable(false)]
                public Task[] TaskElements => GetChildren<Task>();
                [Browsable(false)]
                public PropertyGroup[] PropertyGroupElements => GetChildren<PropertyGroup>();
                [Browsable(false)]
                public ItemGroup[] ItemGroupElements => GetChildren<ItemGroup>();
                [Browsable(false)]
                public OnError[] OnErrorElements => GetChildren<OnError>();

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

                [ElementName("OnError")]
                public class OnError : BaseElement<Target>
                {
                    [Attr("Condition", false)]
                    public string Condition { get; set; }
                    [Attr("ExecuteTargets", true)]
                    public string ExecuteTargets { get; set; }
                }

                [ElementChild(typeof(Output), 0, -1)]
                [ElementName(null)]
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
                    
                    [ElementName("Output")]
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
        /// References an MSBuild project SDK. This element can be used as an alternative to the Sdk attribute.
        /// </summary>
        [ElementName("Sdk")]
        public class Sdk : BaseElement<Project>
        {
            /// <summary>
            /// Required attribute. The name of the project SDK.
            /// </summary>
            [Attr("Name", true)]
            public string Name { get; set; }
            /// <summary>
            /// Optional attribute. The version of the project SDK.
            /// </summary>
            [Attr("Version", false)]
            public string Version { get; set; }
        }
        /// <summary>
        /// A grouping element for individual properties. Properties are specified by using the Property element. There may be zero or more PropertyGroup elements in a project.
        /// </summary>
        [ElementChild(typeof(Property), 0, -1)]
        [ElementName("PropertyGroup")]
        public class PropertyGroup : BaseElement<IPropertyGroupOwner>
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }

            /// <summary>
            /// A user defined property name, which contains the property value. There may be zero or more Property elements in a PropertyGroup element.
            /// </summary>
            [ElementName(null)]
            public class Property : BaseStringElement<PropertyGroup, ElementString>
            {
                [Attr("Condition", false)]
                public string Condition { get; set; }
            }
        }
        /// <summary>
        /// Provides a way to register tasks in MSBuild. There may be zero or more UsingTask elements in a project.
        /// </summary>
        [ElementName("UsingTask")]
        public class UsingTask : BaseElement<IUsingTaskOwner>
        {

        }
        /// <summary>
        /// Provides a way to persist non-MSBuild information in an MSBuild project file. There may be zero or one ProjectExtensions elements in a project.
        /// </summary>
        [ElementName("ProjectExtensions")]
        public class ProjectExtensions : BaseElement<IProjectExtensionsOwner>
        {

        }
        /// <summary>
        /// A grouping element for individual items. Items are specified by using the Item element. There may be zero or more ItemGroup elements in a project.
        /// </summary>
        [ElementChild(typeof(Item), 0, -1)]
        [ElementName("ItemGroup")]
        public class ItemGroup : BaseElement<IItemGroupOwner>, IItemOwner
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }
        /// <summary>
        /// A grouping element for individual items. Items are specified by using the Item element. There may be zero or more ItemDefinitionGroup elements in a project.
        /// </summary>
        [ElementChild(typeof(Item), 0, -1)]
        [ElementName("ItemDefinitionGroup")]
        public class ItemDefinitionGroup : BaseElement<Project>, IItemOwner
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }
        [ElementChild(typeof(ItemMetadata), 0, -1)]
        [ElementName(null)]
        public class Item : BaseElement<IItemOwner>
        {
            /// <summary>
            /// The file or wildcard to include in the list of items.
            /// </summary>
            [Attr("Include", true)]
            public string Include { get; set; }
            /// <summary>
            /// The file or wildcard to exclude from the list of items.
            /// </summary>
            [Attr("Exclude", false)]
            public string Exclude { get; set; }
            /// <summary>
            /// The condition to be evaluated.
            /// For more information, see https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-conditions.
            /// </summary>
            [Attr("Condition", false)]
            public string Condition { get; set; }
            /// <summary>
            /// The file or wildcard to remove from the list of items.
            /// This attribute is valid only if it's specified for an item in an ItemGroup that's in a Target.
            /// </summary>
            [Attr("Remove", false)]
            public string Remove { get; set; }
            /// <summary>
            /// The metadata for the source items to add to the target items.
            /// Only the metadata whose names are specified in the semicolon-delimited list are transferred from a source item to a target item.
            /// For more information, see https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-items.
            /// This attribute is valid only if it's specified for an item in an ItemGroup that's in a Target.
            /// </summary>
            [Attr("KeepMetadata", false)]
            public string KeepMetadata { get; set; }
            /// <summary>
            /// Optional attribute.
            /// The metadata for the source items to not transfer to the target items.
            /// All metadata is transferred from a source item to a target item except metadata whose names are contained in the semicolon-delimited list of names.
            /// For more information, see https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-items.
            /// This attribute is valid only if it's specified for an item in an ItemGroup that's in a Target.
            /// </summary>
            [Attr("RemoveMetadata", false)]
            public string RemoveMetadata { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [Attr("KeepDuplicates", false)]
            public string KeepDuplicates { get; set; }

            [ElementName(null)]
            public class ItemMetadata : BaseStringElement<Item, ElementString>
            {
                [Attr("Condition", false)]
                public string Condition { get; set; }
            }
        }
        /// <summary>
        /// A grouping element for individual items. Items are specified by using the Item element. There may be zero or more ItemGroup elements in a project.
        /// </summary>
        [ElementName("ImportGroup")]
        [ElementChild(typeof(Import), 0, -1)]
        public class ImportGroup : BaseElement<Project>, IImportOwner
        {
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }
        /// <summary>
        /// Enables a project file to import another project file. There may be zero or more Import elements in a project.
        /// </summary>
        [ElementName("Import")]
        public class Import : BaseElement<IImportOwner>
        {
            /// <summary>
            /// Required attribute.
            /// The path of the project file to import.The path can include wildcards.
            /// The matching files are imported in sorted order.
            /// By using this feature, you can add code to a project just by adding the code file to a directory.
            /// </summary>
            [Attr("Project", true)]
            public string Project { get; set; }
            /// <summary>
            /// Optional attribute.
            /// A condition to be evaluated.For more information, see https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-conditions.
            /// </summary>
            [Attr("Condition", false)]
            public string Condition { get; set; }
        }
        /// <summary>
        /// Evaluates child elements to select one set of ItemGroup elements and/or PropertyGroup elements to evaluate.
        /// </summary>
        [ElementName("Choose")]
        [ElementChild(typeof(When), 1, -1)]
        [ElementChild(typeof(Otherwise), 0, 1)]
        public class Choose : BaseElement<IChooseOwner> { }
        /// <summary>
        /// Optional element.
        /// Specifies the block of code PropertyGroup and ItemGroup elements to evaluate if the conditions of all When elements evaluate to false.
        /// There may be zero or one Otherwise elements in a Choose element, and it must be the last element.
        /// </summary>
        [ElementName("Otherwise")]
        [ElementChild(typeof(Choose), 0, -1)]
        [ElementChild(typeof(PropertyGroup), 0, -1)]
        [ElementChild(typeof(ItemGroup), 0, -1)]
        public class Otherwise : BaseElement<Choose>, IChooseOwner, IPropertyGroupOwner, IItemGroupOwner { }
        /// <summary>
        /// Required element. Specifies a possible block of code for the Choose element to select.There may be one or more When elements in a Choose element.
        /// </summary>
        [ElementName("When")]
        [ElementChild(typeof(Choose), 0, -1)]
        [ElementChild(typeof(PropertyGroup), 0, -1)]
        [ElementChild(typeof(ItemGroup), 0, -1)]
        public class When : BaseElement<Choose>, IChooseOwner, IPropertyGroupOwner, IItemGroupOwner
        {
            [Attr("Condition", true)]
            public string Condition { get; set; }
        }
    }
}
