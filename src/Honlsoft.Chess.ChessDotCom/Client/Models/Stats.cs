// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Honlsoft.Chess.ChessDotCom.Models {
    public class Stats : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The best property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public Stats_best? Best { get; set; }
#nullable restore
#else
        public Stats_best Best { get; set; }
#endif
        /// <summary>The last property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public Stats_last? Last { get; set; }
#nullable restore
#else
        public Stats_last Last { get; set; }
#endif
        /// <summary>The record property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public Stats_record? Record { get; set; }
#nullable restore
#else
        public Stats_record Record { get; set; }
#endif
        /// <summary>The tournament property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public Stats_tournament? Tournament { get; set; }
#nullable restore
#else
        public Stats_tournament Tournament { get; set; }
#endif
        /// <summary>
        /// Instantiates a new stats and sets the default values.
        /// </summary>
        public Stats() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static Stats CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new Stats();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"best", n => { Best = n.GetObjectValue<Stats_best>(Stats_best.CreateFromDiscriminatorValue); } },
                {"last", n => { Last = n.GetObjectValue<Stats_last>(Stats_last.CreateFromDiscriminatorValue); } },
                {"record", n => { Record = n.GetObjectValue<Stats_record>(Stats_record.CreateFromDiscriminatorValue); } },
                {"tournament", n => { Tournament = n.GetObjectValue<Stats_tournament>(Stats_tournament.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<Stats_best>("best", Best);
            writer.WriteObjectValue<Stats_last>("last", Last);
            writer.WriteObjectValue<Stats_record>("record", Record);
            writer.WriteObjectValue<Stats_tournament>("tournament", Tournament);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
