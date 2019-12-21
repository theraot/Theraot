#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed partial class InstructionList
    {
        private static Instruction[]? _assignLocal;
        private static Instruction[]? _assignLocalBoxed;
        private static Instruction[]? _assignLocalToClosure;
        private static Instruction? _false;
        private static Instruction[]? _ints;
        private static Instruction[]? _loadLocal;
        private static Instruction[]? _loadLocalBoxed;
        private static Instruction[]? _loadLocalFromClosure;
        private static Instruction[]? _loadLocalFromClosureBoxed;
        private static Instruction[]? _loadObjectCached;
        private static Instruction? _null;
        private static Instruction[]? _storeLocal;
        private static Instruction[]? _storeLocalBoxed;
        private static Instruction? _true;

        public void EmitAssignLocal(int index)
        {
            var assignLocal = GetAssignLocal();
            if (index < assignLocal.Length)
            {
                Emit(assignLocal[index] ??= new AssignLocalInstruction(index));
            }
            else
            {
                Emit(new AssignLocalInstruction(index));
            }
        }

        public void EmitAssignLocalToClosure(int index)
        {
            var assignLocalToClosure = GetAssignLocalToClosure();
            if (index < assignLocalToClosure.Length)
            {
                Emit(assignLocalToClosure[index] ??= new AssignLocalToClosureInstruction(index));
            }
            else
            {
                Emit(new AssignLocalToClosureInstruction(index));
            }
        }

        public void EmitLoad(object? value, Type? type)
        {
            if (value == null)
            {
                Emit(GetNull());
                return;
            }
            if (type?.IsValueType != false)
            {
                switch (value)
                {
                    case bool b:
                        EmitLoad(b);
                        return;

                    case int i when i >= _pushIntMinCachedValue && i <= _pushIntMaxCachedValue:
                        var ints = GetInts();
                        i -= _pushIntMinCachedValue;
                        Emit(ints[i] ??= new LoadObjectInstruction(i));
                        return;

                    default:
                        break;
                }
            }
            _objects ??= new List<object>();
            var loadObjectCached = GetLoadObjectCached();
            if (_objects.Count < loadObjectCached!.Length)
            {
                var index = (uint)_objects.Count;
                _objects.Add(value);
                Emit(loadObjectCached[index] ??= new LoadCachedObjectInstruction(index));
            }
            else
            {
                Emit(new LoadObjectInstruction(value));
            }
        }

        public void EmitLoad(bool value)
        {
            Emit(value ? GetTrue() : GetFalse());
        }

        public void EmitLoadLocal(int index)
        {
            var loadLocal = GetLoadLocal();
            if (index < loadLocal.Length)
            {
                Emit(loadLocal[index] ??= new LoadLocalInstruction(index));
            }
            else
            {
                Emit(new LoadLocalInstruction(index));
            }
        }

        public void EmitLoadLocalFromClosure(int index)
        {
            var loadLocalFromClosure = GetLoadLocalFromClosure();
            if (index < loadLocalFromClosure.Length)
            {
                Emit(loadLocalFromClosure[index] ??= new LoadLocalFromClosureInstruction(index));
            }
            else
            {
                Emit(new LoadLocalFromClosureInstruction(index));
            }
        }

        public void EmitLoadLocalFromClosureBoxed(int index)
        {
            var loadLocalFromClosureBoxed = GetLoadLocalFromClosureBoxed();
            if (index < loadLocalFromClosureBoxed.Length)
            {
                Emit(loadLocalFromClosureBoxed[index] ??= new LoadLocalFromClosureBoxedInstruction(index));
            }
            else
            {
                Emit(new LoadLocalFromClosureBoxedInstruction(index));
            }
        }

        public void EmitStoreLocal(int index)
        {
            var storeLocal = GetStoreLocal();
            if (index < storeLocal.Length)
            {
                Emit(storeLocal[index] ??= new StoreLocalInstruction(index));
            }
            else
            {
                Emit(new StoreLocalInstruction(index));
            }
        }

        internal static Instruction AssignLocalBoxed(int index)
        {
            var assignLocalBoxed = GetAssignLocalBoxed();
            if (index < assignLocalBoxed.Length)
            {
                return assignLocalBoxed[index] ??= new AssignLocalBoxedInstruction(index);
            }
            return new AssignLocalBoxedInstruction(index);
        }

        internal static Instruction LoadLocalBoxed(int index)
        {
            var loadLocalBoxed = GetLoadLocalBoxed();
            if (index < loadLocalBoxed.Length)
            {
                return loadLocalBoxed[index] ??= new LoadLocalBoxedInstruction(index);
            }
            return new LoadLocalBoxedInstruction(index);
        }

        internal static Instruction StoreLocalBoxed(int index)
        {
            var storeLocalBoxed = GetStoreLocalBoxed();
            if (index < storeLocalBoxed.Length)
            {
                return storeLocalBoxed[index] ??= new StoreLocalBoxedInstruction(index);
            }

            return new StoreLocalBoxedInstruction(index);
        }

        private static Instruction[] GetAssignLocal()
        {
            return _assignLocal ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction[] GetAssignLocalBoxed()
        {
            return _assignLocalBoxed ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction[] GetAssignLocalToClosure()
        {
            return _assignLocalToClosure ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction GetFalse()
        {
            return _false ??= new LoadObjectInstruction(Utils.BoxedFalse);
        }

        private static Instruction[] GetInts()
        {
            return _ints ??= new Instruction[_pushIntMaxCachedValue - _pushIntMinCachedValue + 1];
        }

        private static Instruction[] GetLoadLocal()
        {
            return _loadLocal ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction[] GetLoadLocalBoxed()
        {
            return _loadLocalBoxed ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction[] GetLoadLocalFromClosure()
        {
            return _loadLocalFromClosure ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction[] GetLoadLocalFromClosureBoxed()
        {
            return _loadLocalFromClosureBoxed ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction[] GetLoadObjectCached()
        {
            return _loadObjectCached ??= new Instruction[_cachedObjectCount];
        }

        private static Instruction GetNull()
        {
            return _null ??= new LoadObjectInstruction(null);
        }

        private static Instruction[] GetStoreLocal()
        {
            return _storeLocal ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction[] GetStoreLocalBoxed()
        {
            return _storeLocalBoxed ??= new Instruction[_localInstrCacheSize];
        }

        private static Instruction GetTrue()
        {
            return _true ??= new LoadObjectInstruction(Utils.BoxedTrue);
        }
    }
}

#endif