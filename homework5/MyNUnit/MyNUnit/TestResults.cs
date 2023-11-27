using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    public class TestResults
    {
        public string MethodName { get; set; }
        
        public bool IsIgnored { get; set; } = false;

        public string? IgnoreReason { get; set; }

        public Type? ExpectedException { get; set; }

        public Type? ActualException { get; set; }

        public bool IsSuccessful { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        /// <summary>
        /// Constructor for ignored test methods.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreReason"></param>
        public TestResults(string name, string? ignoreReason)
        {
            MethodName = name;
            IsIgnored = true;
            IgnoreReason = ignoreReason;
        }

        /// <summary>
        /// Constructor for executed test methods.
        /// </summary>
        public TestResults(string name, bool isSuccessful, Type? expectedException, Type? actualException, TimeSpan executionTime)
        {
            MethodName = name;
            IsSuccessful = isSuccessful;
            ExpectedException = expectedException;
            ActualException = actualException;
            ExecutionTime = executionTime;
        }
    }
}