﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;

namespace TheraEditor
{
    /// <summary>
    /// Proxy that runs methods in the game's domain.
    /// </summary>
    public class EngineDomainProxyEditor : EngineDomainProxy
    {
        public ConcurrentDictionary<TypeProxy, TypeProxy> FullEditorTypes { get; private set; }
        public ConcurrentDictionary<TypeProxy, TypeProxy> InPlaceEditorTypes { get; private set; }

        public void ReloadEditorTypes()
        {
            if (Engine.DesignMode)
                return;

            InPlaceEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>();
            FullEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>();

            Engine.PrintLine("Loading all editor types to property grid in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
            Task propEditorsTask = Task.Run(() =>
            {
                var propControls = AppDomainHelper.FindTypes(x =>
                    !x.IsAbstract &&
                    x.IsSubclassOf(typeof(PropGridItem)),
                    Assembly.GetExecutingAssembly());

                Parallel.ForEach(propControls, AddPropControlEditorType);
            });
            Task fullEditorsTask = Task.Run(() =>
            {
                var fullEditors = AppDomainHelper.FindTypes(x =>
                    !x.IsAbstract &&
                    x.IsSubclassOf(typeof(Form)) &&
                    x.HasCustomAttribute<EditorForAttribute>(),
                    Assembly.GetExecutingAssembly());

                Parallel.ForEach(fullEditors, AddFullEditorType);
            });
            Task.WhenAll(propEditorsTask, fullEditorsTask).ContinueWith(t =>
                Engine.PrintLine("Finished loading all editor types to property grid."));
        }
        private void AddPropControlEditorType(TypeProxy propControlType)
        {
            var attribs = propControlType.GetCustomAttributes<PropGridControlForAttribute>().ToArray();
            if (attribs.Length > 0)
            {
                PropGridControlForAttribute a = attribs[0];
                foreach (Type varType in a.Types)
                {
                    //if (!_inPlaceEditorTypes.ContainsKey(varType))
                    InPlaceEditorTypes.AddOrUpdate(varType, propControlType, (x, y) => propControlType);
                    //else
                    //    throw new Exception("Type " + varType.GetFriendlyName() + " already has control " + propControlType.GetFriendlyName() + " associated with it.");
                }
            }
        }
        private void AddFullEditorType(TypeProxy editorType)
        {
            var attrib = editorType.GetCustomAttribute<EditorForAttribute>();
            foreach (TypeProxy varType in attrib.DataTypes)
            {
                //if (!_fullEditorTypes.ContainsKey(varType))
                FullEditorTypes.AddOrUpdate(varType, editorType, (x, y) => editorType);
                //else
                //    throw new Exception("Type " + varType.GetFriendlyName() + " already has editor " + editorType.GetFriendlyName() + " associated with it.");
            }
        }
        public override void ResetTypeCaches(bool reloadNow = true)
        {
            ReloadEditorTypes();
            base.ResetTypeCaches();
        }
    }
}
