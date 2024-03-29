// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Honlsoft.Chess.ChessDotCom.Player.Item.Games.Archives.Item.Item {
    public class WithMonthGetResponse : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The games property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<WithMonthGetResponse_games>? Games { get; set; }
#nullable restore
#else
        public List<WithMonthGetResponse_games> Games { get; set; }
#endif
        /// <summary>
        /// Instantiates a new WithMonthGetResponse and sets the default values.
        /// </summary>
        public WithMonthGetResponse() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static WithMonthGetResponse CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new WithMonthGetResponse();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"games", n => { Games = n.GetCollectionOfObjectValues<WithMonthGetResponse_games>(WithMonthGetResponse_games.CreateFromDiscriminatorValue)?.ToList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<WithMonthGetResponse_games>("games", Games);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
