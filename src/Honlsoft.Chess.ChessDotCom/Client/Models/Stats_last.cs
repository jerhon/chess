// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Honlsoft.Chess.ChessDotCom.Models {
    public class Stats_last : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The date property</summary>
        public int? Date { get; set; }
        /// <summary>The rating property</summary>
        public int? Rating { get; set; }
        /// <summary>The rd property</summary>
        public int? Rd { get; set; }
        /// <summary>
        /// Instantiates a new stats_last and sets the default values.
        /// </summary>
        public Stats_last() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static Stats_last CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new Stats_last();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"date", n => { Date = n.GetIntValue(); } },
                {"rating", n => { Rating = n.GetIntValue(); } },
                {"rd", n => { Rd = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteIntValue("date", Date);
            writer.WriteIntValue("rating", Rating);
            writer.WriteIntValue("rd", Rd);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
