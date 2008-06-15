using System;

namespace StructureMap.Diagnostics
{
    [Serializable]
    public class DoctorReport
    {
        public string WhatDoIHave = string.Empty;
        public DoctorResult Result;
        public string ErrorMessages;
    }
}