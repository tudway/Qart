﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qart.Core.DataStore
{
    public interface IDataStore
    {
        Stream GetReadStream(string itemId);

        Stream GetWriteStream(string itemId);

        bool Contains(string itemId);

        IEnumerable<string> GetItemIds(string tag);

        IEnumerable<string> GetItemGroups(string group);
    }


    public static class DataStorageExtensions
    {
        /// <summary>
        /// Retrieves content of the requested item. Returns null if an item is missing.
        /// </summary>
        /// <param name="dataStore"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetContent(this IDataStore dataStore, string itemId)
        {
            if (!dataStore.Contains(itemId))
                return null;
            using (var stream = dataStore.GetReadStream(itemId))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void PutContent(this IDataStore dataStore, string itemId, string content)
        {
            using (var stream = dataStore.GetWriteStream(itemId))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(content);
            }
        }

        public static void UsingReadStream(this IDataStore dataStore, string id, Action<Stream> action)
        {
            dataStore.UsingReadStream(id, stream => { action(stream); return true; });
        }

        public static T UsingReadStream<T>(this IDataStore dataStore, string id, Func<Stream, T> action)
        {
            using (var stream = dataStore.GetReadStream(id))
            {
                return action(stream);
            }
        }

        public static void UsingWriteStream(this IDataStore dataStore, string id, Action<Stream> action)
        {
            dataStore.UsingWriteStream(id, stream => { action(stream); return true; });
        }

        public static T UsingWriteStream<T>(this IDataStore dataStore, string id, Func<Stream, T> action)
        {
            using (var stream = dataStore.GetWriteStream(id))
            {
                return action(stream);
            }
        }

        public static IEnumerable<string> GetAllGroups(this IDataStore dataStore)
        {
            return dataStore.GetAllGroups(".");
        }

        public static IEnumerable<string> GetAllGroups(this IDataStore dataStore, string groupId)
        {
            foreach (var group in dataStore.GetItemGroups(groupId))
            {
                var id = Path.Combine(groupId, group);
                yield return id;

                foreach (var subGroup in GetAllGroups(dataStore, id))
                {
                    yield return subGroup;
                }
            }
        }

        public static IEnumerable<string> GetAllIds(this IDataStore dataStore, string relativeId)
        {
            return dataStore.GetAllGroups(".").Select(_ => Path.Combine(_, relativeId)).Where(_ => dataStore.Contains(_));
        }
    }
}
