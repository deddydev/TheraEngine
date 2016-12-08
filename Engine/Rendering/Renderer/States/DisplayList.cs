using System;

namespace CustomEngine.Rendering
{
    //public class DisplayList : BaseRenderState
    //{
    //    public DisplayList(string name) : base(GenType.DisplayList) { _name = name; }

    //    bool _hasStarted = false;
    //    bool _hasFinished = false;

    //    public void Begin()
    //    {
    //        if (_hasStarted && !_hasFinished)
    //            return;

    //        if (_hasFinished)
    //            Delete();

    //        Generate();
    //        Begin(DisplayListMode.Compile);
    //        _hasStarted = true;
    //        _hasFinished = false;
    //    }
    //    public void Begin(DisplayListMode mode)
    //    {
    //        if (_hasStarted && !_hasFinished)
    //            return;

    //        if (_hasFinished)
    //            Delete();

    //        Generate();
    //        Engine.Renderer.BeginDisplayList(BindingId, mode);
    //        _hasStarted = true;
    //        _hasFinished = false;
    //    }
    //    public void End()
    //    {
    //        if (!_hasStarted)
    //            return;
    //        Engine.Renderer.EndDisplayList();
    //        _hasStarted = false;
    //        _hasFinished = true;
    //    }
    //    public void Call()
    //    {
    //        if (!_hasFinished)
    //            return;

    //        Engine.Renderer.CallDisplayList(BindingId);
    //    }
    //}
}
