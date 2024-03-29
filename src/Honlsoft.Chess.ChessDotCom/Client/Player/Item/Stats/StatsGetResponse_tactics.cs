// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Honlsoft.Chess.ChessDotCom.Player.Item.Stats {
    public class StatsGetResponse_tactics : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The highest property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public StatsGetResponse_tactics_highest? Highest { get; set; }
#nullable restore
#else
        public StatsGetResponse_tactics_highest Highest { get; set; }
#endif
        /// <summary>The lowest property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public StatsGetResponse_tactics_lowest? Lowest { get; set; }
#nullable restore
#else
        public StatsGetResponse_tactics_lowest Lowest { get; set; }
#endif
        /// <summary>
        /// Instantiates a new statsGetResponse_tactics and sets the default values.
        /// </summary>
        public StatsGetResponse_tactics() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static StatsGetResponse_tactics CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new StatsGetResponse_tactics();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"highest", n => { Highest = n.GetObjectValue<StatsGetResponse_tactics_highest>(StatsGetResponse_tactics_highest.CreateFromDiscriminatorValue); } },
                {"lowest", n => { Lowest = n.GetObjectValue<StatsGetResponse_tactics_lowest>(StatsGetResponse_tactics_lowest.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<StatsGetResponse_tactics_highest>("highest", Highest);
            writer.WriteObjectValue<StatsGetResponse_tactics_lowest>("lowest", Lowest);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
