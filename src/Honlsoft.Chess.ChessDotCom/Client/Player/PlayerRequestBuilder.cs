// <auto-generated/>
using Honlsoft.Chess.ChessDotCom.Player.Item;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace Honlsoft.Chess.ChessDotCom.Player {
    /// <summary>
    /// Builds and executes requests for operations under \player
    /// </summary>
    public class PlayerRequestBuilder : BaseRequestBuilder {
        /// <summary>Gets an item from the Honlsoft.Chess.ChessDotCom.player.item collection</summary>
        /// <param name="position">The username of the player</param>
        public WithUsernameItemRequestBuilder this[string position] { get {
            var urlTplParams = new Dictionary<string, object>(PathParameters);
            urlTplParams.Add("username", position);
            return new WithUsernameItemRequestBuilder(urlTplParams, RequestAdapter);
        } }
        /// <summary>
        /// Instantiates a new PlayerRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public PlayerRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/player", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new PlayerRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public PlayerRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/player", rawUrl) {
        }
    }
}
