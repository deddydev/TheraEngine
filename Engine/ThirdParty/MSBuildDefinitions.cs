using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models
{
    public class MSBuild
    {
        public interface IProject : IElement, IChooseOwner, IImportOwner, IItemGroupOwner, IProjectExtensionsOwner, IPropertyGroupOwner, ITargetOwner, IUsingTaskOwner
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
        public interface IPropertyGroupOwner : IElement
        {

        }
        public interface ITargetOwner : IElement
        {

        }
        public interface IUsingTaskOwner : IElement
        {

        }

        [Name("Project")]
        [Child(typeof(Choose), 0, -1)]
        [Child(typeof(Import), 0, -1)]
        [Child(typeof(ItemGroup), 0, -1)]
        [Child(typeof(ProjectExtensions), 0, -1)]
        [Child(typeof(PropertyGroup), 0, -1)]
        [Child(typeof(Target), 0, -1)]
        [Child(typeof(UsingTask), 0, -1)]
        public class MSBuildProject : BaseElement<IElement>, IProject
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
        }

        public class UsingTask : BaseElement<IUsingTaskOwner>
        {

        }

        public class Target : BaseElement<ITargetOwner>
        {

        }

        public class PropertyGroup : BaseElement<IPropertyGroupOwner>
        {

        }

        public class ProjectExtensions : BaseElement<IProjectExtensionsOwner>
        {

        }

        public class ItemGroup : BaseElement<IItemGroupOwner>
        {

        }

        public class Import : BaseElement<IImportOwner>
        {

        }

        public class Choose : BaseElement<IChooseOwner>
        {

        }
    }
}
