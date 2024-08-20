using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;


namespace ECARules4AllPack.Clients
{
    public class Update
    {
        [JsonPropertyName("unity_id")]
        public string entity { get; set; }
        [JsonPropertyName("attribute")]
        public string attribute { get; set; }
        [JsonPropertyName("new_value")]
        public object newValue { get; set; }
        /*[JsonIgnore]
        public long ts;
        [JsonIgnore]
        public bool copiedFlag = false;*/

        public Update(string entity, string attribute, object newValue)
        {
            this.entity = entity;
            this.attribute = attribute;
            this.newValue = newValue;
        }
        
        /*public object Clone()
        {
            var copy = new Update(this.entity, this.attribute, this.newValue);
            DateTimeOffset now = DateTimeOffset.Now;
            long unixTimestamp = now.ToUnixTimeSeconds();
            copy.copiedFlag = true;
            copy.ts = unixTimestamp;
            this.ts = unixTimestamp;
            this.copiedFlag = true;
            return copy;
        }*/

        /*public bool Equals(Update other)
        {
            if (other == null)
                return false;
            return entity == other.entity && attribute == other.attribute && newValue == other.newValue && ts == other.ts;
        }*/
    }
    
    public class UpdateQueue
    {
        private Queue<Update> queue = new Queue<Update>();
        //private List<Update> queue = new List<Update>();
        
        public event EventHandler<Update> ItemAdded;
        
        public void Enqueue(Update item)
        {
            queue.Enqueue(item);
            //queue.Add(item);
            OnItemAdded(item);
        }
        
        public Update Dequeue()
        {
            return queue.Dequeue();
        }

        /*public void Remove(Update item)
        {
            if (item.copiedFlag)
            {
                queue.Remove(item);
            }
            else
            {
                throw new Exception("Impossible to remove an item with copiedFlag = false");
            }
        }
        
        public void Remove(IEnumerable<Update> items)
        {
            foreach (var item in items)
            {
                this.Remove(item);
            }
        }

        public void RemoveAll()
        {
            this.queue = new List<Update>();
        }

        public List<Update> GetUpdates()
        {
           var listUpdates = this.queue.Where(item => !item.copiedFlag || item.ts == default(long))
                .Select(item => 
                {
                    item.copiedFlag = true;
                    item.ts = DateTimeOffset.Now.ToUnixTimeSeconds();
                    return (Update)item;
                })
                .ToList();
           return listUpdates;
        }*/

        protected virtual void OnItemAdded(Update item)
        {
            ItemAdded?.Invoke(this, item); //new UpdateAddedEventArgs(item, this));
        }
    }
    
    public abstract class AbstractClientBase
    {
        public string url { get; set; }
        public string token { get; set; }
        public UpdateQueue updates { get; } = new UpdateQueue();
    }
    
    public abstract class AbstractClient<T> : AbstractClientBase where T : class, new()
    {
        private static T _instance;

        protected AbstractClient()
        {
            this.updates.ItemAdded += sendRequestHandler;
        }

        public static T GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }

        protected abstract void sendRequestHandler(object sender, Update newItem);
    }
}
