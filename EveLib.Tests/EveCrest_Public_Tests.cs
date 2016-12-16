﻿using System;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using eZet.EveLib.Core.RequestHandlers;
using eZet.EveLib.EveCrestModule;
using eZet.EveLib.EveCrestModule.Exceptions;
using eZet.EveLib.EveCrestModule.Models.Links;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eZet.EveLib.Test {
    /// <summary>
    ///     Class EveCrest_Public_Tests.
    /// </summary>
    [TestClass]
    public class EveCrest_Public_Tests {
        private const int AllianceId = 434243723; // C C P Alliance

        private const int RegionId = 10000002; // The Forge 

        private const int TypeId = 34; // Tritanium

        private const string Killmail = "30290604/787fb3714062f1700560d4a83ce32c67640b1797";

        private readonly EveCrest _crest = new EveCrest();


        /// <summary>
        ///     Initializes a new instance of the <see cref="EveCrest_Public_Tests" /> class.
        /// </summary>
        public EveCrest_Public_Tests() {
            _crest.EnableAutomaticTokenRefresh = true;
            _crest.RequestHandler.ThrowOnDeprecated = true;
            _crest.RequestHandler.CacheLevel = CacheLevel.Refresh;
        }

        [TestMethod]
        public async Task GetRoot() {
            var root = await _crest.GetRootAsync();
        }

        [TestMethod]
        public async Task GetIcon() {
            var item = (await _crest.GetRoot().QueryAsync(r => r.MarketTypes)).Items.First();
            var icon = _crest.LoadImage(item.Type.Icon);
        }

        [TestMethod]
        public async Task GetNext() {
            var root = await _crest.GetRootAsync();
            var items = await root.QueryAsync(r => r.ItemTypes);
            await items.QueryAsync(i => i.Next);
        }

        [TestMethod]
        public async Task GetAllItems() {
            var alliances = await (await _crest.GetRootAsync()).QueryAsync(r => r.Alliances);
            await Task.WhenAll(alliances.Items.Select(a => _crest.LoadAsync(a)));
            //var list = alliances.Items.Select(alliance => _crest.QueryAsync(alliance)).ToList();

            //Assert.AreEqual(alliances.TotalCount, alliances.AllItems().Count());
        }

        [TestMethod]
        public void TestIEnumerable() {
            var alliances = _crest.GetRoot().Query(r => r.Alliances);
            var n = alliances.Query(r => r.First(a => a.ShortName == "MZR"));
            Assert.AreEqual(alliances.TotalCount, alliances.Count());
        }

        [TestMethod]
        public async Task TestLinkedParameter() {
            var item = _crest.GetRoot().Query(r => r.ItemTypes).First();
            var orders = await _crest.GetRoot()
                .Query(r => r.Regions)
                .Query(r => r.Items.First())
                .QueryAsync(r => r.MarketOrders, item);
        }

        [TestMethod]
        public async Task TestIQueryParameter() {
            var item = _crest.GetRoot().Query(r => r.ItemTypes).Query(r => r.First());
            var orders = await _crest.GetRoot()
                .Query(r => r.Regions)
                .Query(r => r.Items.First())
                .QueryAsync(r => r.MarketOrders, item);
        }

        [TestMethod]
        public async Task GetRegions() {
            var root = await _crest.GetRootAsync();
            var regions = await root.QueryAsync(r => r.Regions);
            var region = regions.Items.First();
            Assert.AreNotEqual(0, region);
            Assert.AreNotEqual("", region.Name);
            Assert.AreNotEqual("", region.Href);
        }

        [TestMethod]
        public async Task GetRegion() {
            var root = await _crest.GetRootAsync();
            var region = await (await root.QueryAsync(r => r.Regions)).QueryAsync(r => r.Items.First());
            Assert.AreNotEqual(0, region.Id);
            Assert.IsNotNull(region.Name);
            Assert.AreNotEqual(0, region.Constellations.Count);
            Assert.IsNotNull(region.MarketBuyOrders);
            Assert.IsNotNull(region.MarketSellOrders);
        }

        [TestMethod]
        public async Task GetKillmail_NoErrors() {
            var data = await _crest.GetKillmailAsync(28694894, "3d9702696cf8e75d6168734ad26a772e17efc9ba");
            Assert.AreEqual(30000131, data.SolarSystem.Id);
            Assert.AreEqual(99000652, data.Victim.Alliance.Id);
        }

        [TestMethod]
        public async Task GetIncursions_NoErrors() {
            var res = await (await _crest.GetRootAsync()).QueryAsync(r => r.Incursions);
            var inc = res.Items.First();
            Assert.IsTrue(inc.InfestedSolarSystems.Any());
            Assert.IsNotNull(inc.AggressorFaction);
        }

        [TestMethod]
        public async Task GetAllianceCollection() {
            var response = await (await _crest.GetRootAsync()).QueryAsync(r => r.Alliances);
            var list = response.ToList();
            Console.WriteLine(list.Count);
        }

        [TestMethod]
        public async Task GetAlliance_NoErrors() {
            var allianceCollection = await (await _crest.GetRootAsync()).QueryAsync(list => list.Alliances);
            var alliance = allianceCollection.Query(r => r.FirstOrDefault(a => a.Id == 99000006));
            Assert.AreEqual(99000006, alliance.Id);
        }

        //[TestMethod]
        //public void CollectionPaging_Manual() {
        //    AllianceCollectionV1 allianceLinks = crest.GetRoot().QueryAsync(r => r.Alliances);
        //    AllianceCollectionV1.Alliance alliance =
        //        allianceLinks.Items.SingleOrDefault(f => f.Alliance.Id == AllianceId).Alliance;
        //    while (alliance == null && allianceLinks.Next != null) {
        //        allianceLinks = allianceLinks.QueryAsync(f => f.Next);
        //        alliance =
        //        allianceLinks.Items.SingleOrDefault(f => f.Alliance.Id == AllianceId).Alliance;
        //    }
        //    Debug.WriteLine(allianceLinks.QueryAsync(f => alliance).Name);
        //}


        [TestMethod]
        public async Task GetMarketHistory_NoErrors() {
            var data = await _crest.GetMarketHistoryAsync(RegionId, TypeId);
        }

        [TestMethod]
        public async Task GetMarketPrices() {
            var result = (await (await _crest.GetRootAsync()).QueryAsync(r => r.MarketPrices));
            Console.WriteLine(result.Items.Count);
        }

        [TestMethod]
        public async Task GetWars_NoErrors() {
            var wars = await (await _crest.GetRootAsync()).QueryAsync(r => r.Wars);
            var war = wars.Items.First();
            Assert.IsNotNull(war);
            Assert.AreNotEqual(0, war.Id);
            Assert.IsNotNull(war.Href);
        }

        [TestMethod]
        public async Task GetWar_NoErrors() {
            var wars = (await (await _crest.GetRootAsync()).QueryAsync(r => r.Wars));
            var war = await wars.QueryAsync(f => f.First());
            Assert.IsNotNull(war.TimeDeclared);
            Assert.IsNotNull(war.TimeFinished);
            Assert.IsNotNull(war.TimeStarted);
            testLinkedEntity(war.Aggressor);
            testLinkedEntity(war.Defender);
        }

        [TestMethod]
        public async Task GetWarKillmails_NoErrors() {
            var war = await (await (await _crest.GetRootAsync()).QueryAsync(r => r.Wars)).QueryAsync(r => r.First());
            var killmails = await war.QueryAsync(r => r.Killmails);
        }

        [TestMethod]
        public async Task GetIndustrySystemsAsync() {
            var result = (await (await _crest.GetRootAsync()).QueryAsync(r => r.Industry.Systems));
            Console.Write(result);
        }

        [TestMethod]
        public async Task GetIndustryFacilities() {
            var result = (await (await _crest.GetRootAsync()).QueryAsync(r => r.Industry.Facilities));
        }

        [TestMethod]
        public async Task ItemGroups() {
            var itemGroups = await _crest.GetRoot().QueryAsync(r => r.ItemGroups);
            var group = itemGroups.Query(f => f.Single(g => g.Id == 354753));
            Console.WriteLine(group.Name);
        }

        [TestMethod]
        public async Task MarketGroups() {
            var result = await _crest.GetRoot().QueryAsync(r => r.MarketGroups);
            var first = result.Items.First();
            Assert.IsNotNull(first.Types);
            Assert.IsNotNull(first.Description);
            testLinkedEntity(first);
        }

        [TestMethod]
        public async Task MarketTypes() {
            var result =
                await _crest.GetRoot().QueryAsync(r => r.MarketTypes);
            var item = result.Items.First();
            Assert.IsNotNull(item.MarketGroup);
            testLinkedEntity(item.Type);

            Assert.AreNotEqual(0, item.MarketGroup.Id);
            Assert.IsNotNull(item.MarketGroup.Href);
        }

        [TestMethod]
        public async Task GetSovereigntyStructures() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Sovereignty.Structures);
        }

        [TestMethod]
        public async Task GetSovereigntyCampaigns() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Sovereignty.Campaigns);
        }

        [TestMethod]
        public async Task GetItemTypes() {
            var response = await _crest.GetRoot().QueryAsync(r => r.ItemTypes);
            Assert.AreNotEqual(0, response.Items.Count);
            testLinkedEntity(response.Items[1]);
        }

        [TestMethod]
        public async Task GetItemType() {
            var response = await _crest.GetRoot().QueryAsync(r => r.ItemTypes);
            var type = response.Query(r => r.Items.Single(t => t.Id == 200));
            Assert.IsNotNull(type.GraphicId);
            Assert.AreNotEqual(0, type.Dogma.Attributes.Count);
            Assert.IsNotNull(type.Dogma.Attributes.First());
            Assert.IsNotNull(type.Dogma.Effects.First());
            Assert.AreNotEqual(0, type.Dogma.Effects.Count);
            testLinkedEntity(type.Dogma.Attributes.First().Attribute);
            testLinkedEntity(type.Dogma.Effects.First().Effect);
        }

        [TestMethod]
        public async Task GetConstellations() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Constellations);
            Assert.AreNotEqual(0, response.Items.Count);
            testLinkedEntity(response.Items.First());
        }


        [TestMethod]
        public async Task GetSystems() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Systems);
            Assert.AreNotEqual(0, response.Items.Count);
            testLinkedEntity(response.Items.First());
        }

        [TestMethod]
        public async Task GetDogmaAttributes() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Dogma.Attributes);
            Assert.AreNotEqual(0, response.Items.Count);
            var item = response.Items.First();
            testLinkedEntity(item);
        }

        [TestMethod]
        public async Task GetDogmaAttribute() {
            var item =
                await (await (_crest.GetRoot().QueryAsync(r => r.Dogma.Attributes))).QueryAsync(r => r.Items.First());
            Assert.IsNotNull(item.DisplayName);
            Assert.IsNotNull(item.Description);
        }

        [TestMethod]
        public async Task GetDogmaEffects() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Dogma.Effects);
            Assert.AreNotEqual(0, response.Items.Count);
            var item = response.Items.First();
            testLinkedEntity(item);
        }

        [TestMethod]
        public async Task GetDogmaEffect() {
            var item =
                await (await (_crest.GetRoot().QueryAsync(r => r.Dogma.Effects))).QueryAsync(r => r.Items.First());
            Assert.IsNotNull(item.Description);
        }

        [TestMethod]
        public async Task GetBuyOrderWith() {
            var orders =
                (await
                    (await
                        (await _crest.GetRootAsync())
                            .QueryAsync(r => r.Regions))
                        .QueryAsync(x => x.Single(r => r.Name == "The Forge")))
                    .Query(r => r.MarketBuyOrders, "type",
                        "https://crest-tq.eveonline.com/inventory/types/34/");
        }

        [TestMethod]
        public async Task GetMarketOrders() {
            var orders =
                (await
                    (await
                        (await _crest.GetRootAsync())
                            .QueryAsync(r => r.Regions))
                        .QueryAsync(x => x.Single(r => r.Name == "The Forge")))
                    .Query(r => r.MarketBuyOrders, "type",
                        "https://crest-tq.eveonline.com/inventory/types/34/");
        }

        private static void testLinkedEntity<T>(ILinkedEntity<T> entity) {
            Assert.IsNotNull(entity);
            Assert.AreNotEqual(0, entity.Id);
            Assert.IsNotNull(entity.Href);
            Assert.IsNotNull(entity.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(EveCrestException))]
        public async Task Time() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Time);
        }


        [TestMethod]
        public void ServerName() {
            Assert.AreEqual("TRANQUILITY", _crest.GetRoot().ServerName);
        }

        [TestMethod]
        public async Task Incursions() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Incursions);
        }

        [TestMethod]
        [ExpectedException(typeof(EveCrestException))]
        public async Task Races() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Races);
        }

        [TestMethod]
        public void VirtualGoodStore() {
            Assert.IsNotNull(_crest.GetRoot().VirtualGoodStore);
        }

        [TestMethod]
        public async Task Tournaments() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Tournaments);
            var tournament = _crest.Load(response.Items.Single(r => r.Id == 14));
            var match = _crest.Load(tournament.Series).Query(r => r.Items.First().Matches).Items.First();
        }

        [TestMethod]
        public void ServerVersion() {
            Assert.IsNotNull(_crest.GetRoot().ServerVersion);
        }

        [TestMethod]
        public void AuthEndpoints() {
            Assert.AreEqual("https://login-tq.eveonline.com/oauth/token/", _crest.GetRoot().AuthEndpoint.Uri);
        }

        [TestMethod]
        public void ServiceStatus() {
            var status = _crest.GetRoot().ServiceStatus;
            //Assert.AreEqual(CrestRoot.ServiceStatusType.Online, status.Dust);
            //Assert.AreEqual(CrestRoot.ServiceStatusType.Online, status.Eve);
            //Assert.AreEqual(CrestRoot.ServiceStatusType.Online, status.Server);
        }

        [TestMethod]
        [ExpectedException(typeof(EveCrestException))]
        public async Task Bloodlines() {
            var response = await _crest.GetRoot().QueryAsync(r => r.Bloodlines);
        }

        [TestMethod]
        public async Task ItemCategories() {
            var response = await _crest.GetRoot().QueryAsync(r => r.ItemCategories);
        }  
        
        [TestMethod]
        public async Task NpcCorporationCollection() {
            var response = await (await _crest.GetRootAsync()).QueryAsync(r => r.NpcCorporations);

        }

        [TestMethod]
        public async Task LoyaltyStoreOffersCollection() {
            var response = await (await _crest.GetRootAsync()).QueryAsync(r => r.NpcCorporations);
            var store = await response.QueryAsync(r => r.Skip(1).First().LoyaltyStore);
        }

        [TestMethod]
        public async Task Station() {
            var response = await (await _crest.GetRootAsync()).QueryAsync(r => r.NpcCorporations);
            var station = await response.QueryAsync(r => r.Skip(1).First().HeadQuarters);
        }

    }
}