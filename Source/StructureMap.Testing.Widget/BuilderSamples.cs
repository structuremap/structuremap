using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Widget
{
    
    public enum Color
    {
        Red,
        Blue,
        Green
    }

    public class SetterTarget
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
        public bool Active { get; set; }
        public Color Color { get; set; }
    }



    //public class BuilderWithOneSetter : InstanceBuilder
    //{
    //    public override Type PluggedType
    //    {
    //        get { throw new System.NotImplementedException(); }
    //    }

    //    public override object BuildInstance(IConfiguredInstance instance, BuildSession session)
    //    {
    //        SetterTarget target = new SetterTarget();
    //        if (instance.HasProperty("Name", null)) target.Name = instance.GetProperty("Name");
    //        return target;
    //    }
    //}

    //public class BuilderWithTwoSetters : InstanceBuilder
    //{
    //    public override Type PluggedType
    //    {
    //        get { throw new System.NotImplementedException(); }
    //    }

    //    public override object BuildInstance(IConfiguredInstance instance, BuildSession session)
    //    {
    //        SetterTarget target = new SetterTarget();
    //        if (instance.HasProperty("Name", null)) target.Name = instance.GetProperty("Name");
    //        if (instance.HasProperty("Name2", null)) target.Name = instance.GetProperty("Name2");
    //        return target;
    //    }

    //    public override void BuildUp(IConfiguredInstance instance, BuildSession session, object target)
    //    {
    //        SetterTarget x = (SetterTarget) target;
    //        if (instance.HasProperty("Name", null)) x.Name = instance.GetProperty("Name");
    //        if (instance.HasProperty("Name2", null)) x.Name = instance.GetProperty("Name2");
    //    }
    //}
}
