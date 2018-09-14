﻿//------------------------------------------------------------------------------
// <copyright file="ExceptionHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">nzeng</owner>
// Security review: nzeng 01-11-06
//------------------------------------------------------------------------------

#region Using directives

using System;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Microsoft.Hpc
{

    public static class ExceptionHelper
    {

        public static bool IsCatchableException(Exception e)
        {
            Debug.Assert(e != null, "Unexpected null exception");
            return !(
                e is StackOverflowException ||
                e is OutOfMemoryException ||
                e is ThreadAbortException ||
                e is ThreadInterruptedException ||
                e is NullReferenceException ||
                e is AccessViolationException ||
                e is TaskCanceledException ||
                e is OperationCanceledException
            );
        }
    }
}