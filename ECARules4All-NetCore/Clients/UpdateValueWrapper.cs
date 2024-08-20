using System;
using UnityEngine;
using System.Text.Json;


namespace ECARules4AllPack.Clients
{
    public static class UpdateValueWrapper
    {
        public static void UpdateValue<T>(string ownerName, string propertyName, T newValue)
        {
            foreach (var client in RuleEngine.GetInstance().clients)
            {
                client.updates.Enqueue(new Update(ownerName, propertyName, newValue));
            }
            Debug.Log($"new value: {newValue}");
        }
    }
}
