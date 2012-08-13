using System;

namespace StructureMap.Diagnostics
{
    [Serializable]
    public class DoctorReport
    {
        public string ErrorMessages;
        public DoctorResult Result;
        public string WhatDoIHave = string.Empty;
    }
}