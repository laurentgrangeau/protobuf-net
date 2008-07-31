﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBuf
{
    internal sealed class ListProperty<TEntity, TList, TValue> : PropertyBase<TEntity, TList, TValue>
        where TEntity : class, new()
        where TList : class, IList<TValue>
    {
        public ListProperty(PropertyInfo property)
            : base(property)
        {}

        public override bool IsRepeated { get { return true; } }

        public override int Serialize(TList list, SerializationContext context)
        { // write all items in a contiguous block
            int total = 0;
            foreach (TValue value in list)
            {
                total += SerializeValue(value, context);
            }
            return total;
        }
        protected override bool HasValue(TList list)
        {
            return list != null && list.Count > 0;
        }
        protected override int GetLengthImpl(TList list, SerializationContext context)
        {
            int total = 0;
            foreach (TValue value in list)
            {
                total += GetValueLength(value, context);
            }
            return total;
        }
        private void AddItem(TEntity instance, TValue value)
        {
            TList list = GetValue(instance);
            bool set = list == null;
            if (set) list = (TList)Activator.CreateInstance(typeof(TList));
            list.Add(value);
            if (set) SetValue(instance, list);
        }
        public override void Deserialize(TEntity instance, SerializationContext context)
        {   // read a single item
            TValue value = ValueSerializer.Deserialize(default(TValue), context);
            AddItem(instance, value);
            Trace(true, value, context);
        }

        public override void DeserializeGroup(TEntity instance, SerializationContext context)
        {
            // read a single item
            TValue value = GroupSerializer.DeserializeGroup(default(TValue), context);
            AddItem(instance, value);
            Trace(true, value, context);
        }
    }
}