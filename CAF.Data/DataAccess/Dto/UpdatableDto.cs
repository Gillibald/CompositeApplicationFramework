using System.Xml.Serialization;

namespace CompositeApplicationFramework.DataAccess.Dto
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Interfaces;

    [DataContract]
    public abstract class UpdatableDto<TId> : IDto<TId>, ISupportInitialize
    {
        [DataMember]
        public TId Id { get; set; }
        [DataMember]
        [XmlIgnore]
        public IDictionary<string, object> ExtraElements { get; set; }


        void ISupportInitialize.BeginInit()
        {
            if (ExtraElements == null) ExtraElements = new Dictionary<string, object>();
        }

        void ISupportInitialize.EndInit()
        {
            var removableElements =
                ExtraElements.Where(extraElement => MapExtraElement(extraElement.Key, extraElement.Value)).ToList()
                    .Select(extraElement => extraElement.Key);

            foreach (var removableElement in removableElements)
            {
                ExtraElements.Remove(removableElement);
            }
        }

        protected virtual bool MapExtraElement(string key, object value)
        {
            return false;
        }
    }
}
