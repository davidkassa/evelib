﻿using System;
using eZet.Eve.EoLib.Dto.EveApi;
using eZet.Eve.EoLib.Dto.EveApi.Core;
using CharacterInfo = eZet.Eve.EoLib.Dto.EveApi.Core.CharacterInfo;

namespace eZet.Eve.EoLib.Entity {

    /// <summary>
    /// Provides access to API calls that do not require a valid API key or character id. That is, all URIs prefixed with /eve, /server and /api.
    /// </summary>
    public class Core : BaseEntity {

        /// <summary>
        /// The base URI for all requests by this entity
        /// </summary>
        protected override sealed string UriBase { get; set; }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        internal Core() {
            UriBase = "https://api.eveonline.com";
        }

        /// <summary>
        /// Returns a list of alliances in eve.
        /// </summary>
        /// <param name="extended">Optional; If true, includes corporations.</param>
        /// <returns></returns>
        public XmlResponse<AllianceList> GetAllianceList(bool extended = false) {
            const string relPath = "/eve/AllianceList.xml.aspx";
            var postString = extended ? "version=2" : "version=1";
            return request(relPath, new AllianceList(), postString);
        }

        /// <summary>
        /// Returns a list of certificates in eve.
        /// </summary>
        /// <returns></returns>
        public XmlResponse<CertificateTree> GetCertificateTree() {
            const string relPath = "/eve/CertificateTree.xml.aspx";
            return request(relPath, new CertificateTree());
        }

        /// <summary>
        /// Returns the characterName, characterID, corporationName, corporationID, allianceName, allianceID, factionName, factionID for the given list of IDs.
        /// </summary>
        /// <param name="list">A list of character ids.</param>
        /// <returns></returns>
        public XmlResponse<CharacterAffiliation> GetCharacterAffiliation(params long[] list) {
            const string relPath = "/eve/CharacterAffiliation.xml.aspx";
            var ids = String.Join(",", list);
            var postString = RequestHelper.GeneratePostString("ids", ids);
            return request(relPath, new CharacterAffiliation(), postString);
        }

        /// <summary>
        /// Returns the ownerID for a given character, faction, alliance or corporation name, or the typeID for other objects such as stations, solar systems, planets, etc.
        /// </summary>
        /// <param name="list">A list of ids.</param>
        /// <returns></returns>
        public XmlResponse<CharacterNameId> GetCharacterId(params string[] list) {
            const string relPath = "/eve/CharacterID.xml.aspx";
            var names = String.Join(",", list);
            var postString = RequestHelper.GeneratePostString("names", names);
            return request(relPath, new CharacterNameId(), postString);
        }

        /// <summary>
        /// Returns the same data as a show info call on the character would do in the client. For the extended API key version, see Character.GetCharacterInfo.
        /// </summary>
        /// <param name="id">The character id.</param>
        /// <returns></returns>
        public XmlResponse<CharacterInfo> GetCharacterInfo(long id) {
            const string relPath = "/eve/CharacterInfo.xml.aspx";
            var postString = RequestHelper.GeneratePostString("characterID", id);
            return request(relPath, new CharacterInfo(), postString);
        }

        /// <summary>
        /// Returns the name associated with an ownerID.
        /// <para></para>
        /// A hard maximum of 250 IDs passed in. Might change in the future depending on live results.
        /// Any instances of repeated ids in the string will throw immediate errors with no returns.
        /// If an ID is passed into the call that does not resolve the call will not return any results regardless of the validity of other ids.
        /// </summary>
        /// <param name="list">List of ownerIDs (characterID, agentID, corporationID, allianceID, or factionID) and typeIDs to query.</param>
        /// <returns></returns>
        public XmlResponse<CharacterNameId> GetCharacterName(params long[] list) {
            const string relPath = "/eve/CharacterName.xml.aspx";
            var ids = String.Join(",", list);
            var postString = RequestHelper.GeneratePostString("ids", ids);
            return request(relPath, new CharacterNameId(), postString);
        }

        /// <summary>
        /// Returns a list of conquerable stations
        /// </summary>
        /// <returns></returns>
        public XmlResponse<ConquerableStations> GetConquerableStations() {
            const string relPath = "/eve/ConquerableStationList.xml.aspx";
            return request(relPath, new ConquerableStations());
        }

        /// <summary>
        /// Returns a list of error codes that can be returned by the EVE API servers. Error types are broken into the following categories according to their first digit:
        /// 1xx - user input
        /// 2xx - authentication
        /// 5xx - server
        /// 9xx - miscellaneous
        /// </summary>
        /// <returns></returns>
        public XmlResponse<ErrorList> GetErrorList() {
            const string relPath = "/eve/ErrorList.xml.aspx";
            return request(relPath, new ErrorList());
        }

        /// <summary>
        /// Returns global stats on the factions in factional warfare
        /// </summary>
        /// <returns></returns>
        public XmlResponse<FactionWarfareStats> GetFactionWarfareStats() {
            const string relPath = "/eve/FacWarStats.xml.aspx";
            return request(relPath, new FactionWarfareStats());
        }

        /// <summary>
        /// Returns Factional Warfare Top 100 Stats
        /// </summary>
        /// <returns></returns>
        public XmlResponse<FactionWarTopStats> GetFactionWarfareTopList() {
            const string relPath = "/eve/FacWarTopStats.xml.aspx";
            return request(relPath, new FactionWarTopStats());
        }

        /// <summary>
        /// Returns the transaction types used in GetWalletJournal calls.
        /// </summary>
        /// <returns></returns>
        public XmlResponse<ReferenceTypes> GetReferenceTypes() {
            const string relPath = "/eve/RefTypes.xml.aspx";
            return request(relPath, new ReferenceTypes());
        }

        /// <summary>
        /// Returns the current in-game skills (including unpublished skills).
        /// </summary>
        /// <returns></returns>
        public XmlResponse<SkillTree> GetSkillTree() {
            const string relPath = "/eve/SkillTree.xml.aspx";
            return request(relPath, new SkillTree());
        }

        /// <summary>
        /// Returns the name associated with a typeID.
        /// </summary>
        /// <param name="list">A list of type ids.</param>
        /// <returns></returns>
        public XmlResponse<TypeName> GetTypeName(params long[] list) {
            const string relPath = "/eve/TypeName.xml.aspx";
            var ids = String.Join(",", list);
            var postString = RequestHelper.GeneratePostString("ids", ids);
            return request(relPath, new TypeName(), postString);
        }

        /// <summary>
        /// Returns current Tranquility status and number of players online.
        /// </summary>
        /// <returns></returns>
        public XmlResponse<ServerStatus> GetServerStatus() {
            const string relPath = "/server/ServerStatus.xml.aspx";
            return request(relPath, new ServerStatus());
        }

        /// <summary>
        /// Returns the mask and groupings for calls under the new Customizable API Keys authentication method.
        /// </summary>
        /// <returns></returns>
        public XmlResponse<CallList> GetCallList() {
            // TODO Separate rowsets
            const string relPath = "/api/calllist.xml.aspx";
            return request(relPath, new CallList());
        }
    }
}