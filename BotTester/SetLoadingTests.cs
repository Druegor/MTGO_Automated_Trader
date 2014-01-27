using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CardDataLayer;
using NUnit.Framework;
using CardSet = CardDataLayer.CardSet;

namespace BotTester
{
    [TestFixture]
    public class SetLoadingTests
    {
        private string sets =
            "<Sets>"
            + "<Set ShortName=\"FK\"    LongName=\"FK\"		                            DataRef=\"DOSets\\sFK.dat\"/>"
            + "<Set ShortName=\"EVENT\" LongName=\"EVENT\"				                      DataRef=\"DOSets\\sEVENT.dat\"/>"
            + "<Set ShortName=\"CC\"    LongName=\"CC\"						                    DataRef=\"DOSets\\sCC.dat\"/>"
            + "<Set ShortName=\"BET\"   LongName=\"Beta Boosters\"                     DataRef=\"DOSets\\sBET.dat\"/>"
            + "<Set ShortName=\"TOK\"   LongName=\"Tokens\"					                  DataRef=\"DOSets\\sTOK.dat\"/>"
            + "<Set ShortName=\"M11\"   LongName=\"Magic 2011\"			                  DataRef=\"DOSets\\sM11.dat\"/>"
            + "<Set ShortName=\"M10\"   LongName=\"Magic 2010\"			                  DataRef=\"DOSets\\sM10.dat\"/>"
            + "<Set ShortName=\"10E\"   LongName=\"Tenth Edition\"			                DataRef=\"DOSets\\s10E.dat\"/>"
            + "<Set ShortName=\"9ED\"   LongName=\"Ninth Edition\"			                DataRef=\"DOSets\\s9ED.dat\"/>"
            + "<Set ShortName=\"8ED\"   LongName=\"Eighth Edition\"			              DataRef=\"DOSets\\s8ED.dat\"/>"
            + "<Set ShortName=\"7E\"    LongName=\"Seventh Edition\"			              DataRef=\"DOSets\\s7E.dat\"/>"
            + "<Set ShortName=\"DPA\"   LongName=\"Planeswalker\"                      DataRef=\"DOSets\\sDPA.dat\"/>"
            + "<Set ShortName=\"F09\"   LongName=\"Shards of Alara Block (Standard)\"  DataRef=\"DOSets\\sF09.dat\"/>"
            + "<Set ShortName=\"F10\"   LongName=\"Shards of Alara Block (Premium)\"   DataRef=\"DOSets\\sF10.dat\"/>"
            + "<Set ShortName=\"H09\"   LongName=\"Slivers\"                           DataRef=\"DOSets\\sH09.dat\"/>"
            + "<Set ShortName=\"PD2\"   LongName=\"Fire &amp; Lightning\"              DataRef=\"DOSets\\sPD2.dat\"/>"
            + "<Set ShortName=\"TD0\"   LongName=\"Theme Deck Set\"                    DataRef=\"DOSets\\sTD0.dat\"/>"
            + "<Set ShortName=\"TD2\"   LongName=\"Mirrodin Pure vs New Phyrexia\"                    DataRef=\"DOSets\\sTD2.dat\"/>"
            + "<Set ShortName=\"DD2\"   LongName=\"Jace vs Chandra\"                   DataRef=\"DOSets\\sDD2.dat\"/>"
            + "<Set ShortName=\"DDC\"   LongName=\"Divine vs Demonic\"                 DataRef=\"DOSets\\sDDC.dat\"/>"
            + "<Set ShortName=\"DDD\"   LongName=\"Garruk vs Liliana\"                 DataRef=\"DOSets\\sDDD.dat\"/>"
            + "<Set ShortName=\"DDE\"   LongName=\"Phyrexia vs The Coalition\"         DataRef=\"DOSets\\sDDE.dat\"/>"
            + "<Set ShortName=\"DDF\"   LongName=\"Elspeth vs Tezzeret\"               DataRef=\"DOSets\\sDDF.dat\"/>"
            + "<Set ShortName=\"DDG\"   LongName=\"Knighs vs Dragons\"                 DataRef=\"DOSets\\sDDG.dat\"/>"
            + "<Set ShortName=\"DRB\"   LongName=\"Dragons\"                           DataRef=\"DOSets\\sDRB.dat\"/>"
            + "<Set ShortName=\"CMD\"   LongName=\"Commander\"                         DataRef=\"DOSets\\sCMD.dat\"/>"
            + "<Set ShortName=\"V09\"   LongName=\"Exiled\"                            DataRef=\"DOSets\\sV09.dat\"/>"
            + "<Set ShortName=\"V10\"   LongName=\"Relics\"                            DataRef=\"DOSets\\sV10.dat\"/>"
            + "<Set ShortName=\"EVG\"   LongName=\"Elves vs Goblins\"                  DataRef=\"DOSets\\sEVG.dat\"/>"
            + "<Set ShortName=\"ME4\"   LongName=\"Masters Edition IV\"                DataRef=\"DOSets\\sME4.dat\"/>"
            + "<Set ShortName=\"ME3\"   LongName=\"Masters Edition III\"               DataRef=\"DOSets\\sME3.dat\"/>"
            + "<Set ShortName=\"ME2\"   LongName=\"Masters Edition II\"                DataRef=\"DOSets\\sME2.dat\"/>"
            + "<Set ShortName=\"MED\"   LongName=\"Masters Edition\"                   DataRef=\"DOSets\\sMED.dat\"/>"
            + "<Set ShortName=\"NPH\"   LongName=\"New Phyrexia\"                 DataRef=\"DOSets\\sNPH.dat\"/>"
            + "<Set ShortName=\"MB2\"   LongName=\"Mirrodin Besieged Phyrexian\"       DataRef=\"DOSets\\sMB2.dat\"/>"
            + "<Set ShortName=\"MB3\"   LongName=\"Mirrodin Besieged Mirran\"          DataRef=\"DOSets\\sMB3.dat\"/>"
            + "<Set ShortName=\"MBS\"   LongName=\"Mirrodin Besieged\"                 DataRef=\"DOSets\\sMBS.dat\"/>"
            + "<Set ShortName=\"SOM\"   LongName=\"Scars of Mirrodin\"                 DataRef=\"DOSets\\sSOM.dat\"/>"
            + "<Set ShortName=\"ZEN\"   LongName=\"Zendikar\"                          DataRef=\"DOSets\\sZEN.dat\"/>"
            + "<Set ShortName=\"WWK\"   LongName=\"Worldwake\"                         DataRef=\"DOSets\\sWWK.dat\"/>"
            + "<Set ShortName=\"ROE\"   LongName=\"Rise of the Eldrazi\"               DataRef=\"DOSets\\sROE.dat\"/>"
            + "<Set ShortName=\"ALA\"   LongName=\"Shards of Alara\"                   DataRef=\"DOSets\\sALA.dat\"/>"
            + "<Set ShortName=\"CON\"   LongName=\"Conflux\"                           DataRef=\"DOSets\\sCON.dat\"/>"
            + "<Set ShortName=\"ARB\"   LongName=\"Alara Reborn\"                      DataRef=\"DOSets\\sARB.dat\"/>"
            + "<Set ShortName=\"SHM\"   LongName=\"Shadowmoor\"                        DataRef=\"DOSets\\sSHM.dat\"/>"
            + "<Set ShortName=\"EVE\"   LongName=\"Eventide\"                          DataRef=\"DOSets\\sEVE.dat\"/>"
            + "<Set ShortName=\"LRW\"   LongName=\"Lorwyn\"                            DataRef=\"DOSets\\sLRW.dat\"/>"
            + "<Set ShortName=\"MOR\"   LongName=\"Morningtide\"                       DataRef=\"DOSets\\sMOR.dat\"/>"
            + "<Set ShortName=\"TSP\"   LongName=\"Time Spiral\"                       DataRef=\"DOSets\\sTSP.dat\"/>"
            + "<Set ShortName=\"TSB\"   LongName=\"Timeshifted\"				                DataRef=\"DOSets\\sTSB.dat\"/>"
            + "<Set ShortName=\"PLC\"   LongName=\"Planar Chaos\"                      DataRef=\"DOSets\\sPLC.dat\"/>"
            + "<Set ShortName=\"FUT\"   LongName=\"Future Sight\"                      DataRef=\"DOSets\\sFUT.dat\"/>"
            + "<Set ShortName=\"ICE\"   LongName=\"Ice Age Reprints\"			            DataRef=\"DOSets\\sICE.dat\"/>"
            + "<Set ShortName=\"ALL\"   LongName=\"Alliances Reprints\"			DataRef=\"DOSets\\sALL.dat\"/>"
            + "<Set ShortName=\"CSP\"   LongName=\"Coldsnap\"			          DataRef=\"DOSets\\sCSP.dat\"/>"
            + "<Set ShortName=\"RAV\"   LongName=\"Ravnica: City of Guilds\"	DataRef=\"DOSets\\sRAV.dat\"/>"
            + "<Set ShortName=\"GPT\"   LongName=\"Guildpact\"				        DataRef=\"DOSets\\sGPT.dat\"/>"
            + "<Set ShortName=\"DIS\"   LongName=\"Dissension\"				      DataRef=\"DOSets\\sDIS.dat\"/>"
            + "<Set ShortName=\"CHK\"   LongName=\"Champions of Kamigawa\"	  DataRef=\"DOSets\\sCHK.dat\"/>"
            + "<Set ShortName=\"BOK\"   LongName=\"Betrayers of Kamigawa\"	  DataRef=\"DOSets\\sBOK.dat\"/>"
            + "<Set ShortName=\"SOK\"   LongName=\"Saviors of Kamigawa\"	    DataRef=\"DOSets\\sSOK.dat\"/>"
            + "<Set ShortName=\"MRD\"   LongName=\"Mirrodin\"				        DataRef=\"DOSets\\sMRD.dat\"/>"
            + "<Set ShortName=\"DST\"   LongName=\"Darksteel\"				        DataRef=\"DOSets\\sDST.dat\"/>"
            + "<Set ShortName=\"5DN\"   LongName=\"Fifth Dawn\"				      DataRef=\"DOSets\\s5DN.dat\"/>"
            + "<Set ShortName=\"ONS\"   LongName=\"Onslaught\"			          DataRef=\"DOSets\\sONS.dat\"/>"
            + "<Set ShortName=\"LGN\"   LongName=\"Legions\"				          DataRef=\"DOSets\\sLGN.dat\"/>"
            + "<Set ShortName=\"SCG\"   LongName=\"Scourge\"				          DataRef=\"DOSets\\sSCG.dat\"/>"
            + "<Set ShortName=\"OD\"    LongName=\"Odyssey\"					        DataRef=\"DOSets\\sOD.dat\"/>"
            + "<Set ShortName=\"TOR\"   LongName=\"Torment\"				          DataRef=\"DOSets\\sTOR.dat\"/>"
            + "<Set ShortName=\"JUD\"   LongName=\"Judgment\"				        DataRef=\"DOSets\\sJUD.dat\"/>"
            + "<Set ShortName=\"IN\"    LongName=\"Invasion\"				        DataRef=\"DOSets\\sIN.dat\"/>"
            + "<Set ShortName=\"PS\"    LongName=\"Planeshift\"			        DataRef=\"DOSets\\sPS.dat\"/>"
            + "<Set ShortName=\"AP\"    LongName=\"Apocalypse\"			        DataRef=\"DOSets\\sAP.dat\"/>"
            + "<Set ShortName=\"TE\"    LongName=\"Tempest\"					        DataRef=\"DOSets\\sTE.dat\"/>"
            + "<Set ShortName=\"ST\"    LongName=\"Stronghold\"			        DataRef=\"DOSets\\sST.dat\"/>"
            + "<Set ShortName=\"EX\"    LongName=\"Exodus\"			            DataRef=\"DOSets\\sEX.dat\"/>"
            + "<Set ShortName=\"MI\"    LongName=\"Mirage\"					        DataRef=\"DOSets\\sMI.dat\"/>"
            + "<Set ShortName=\"VI\"    LongName=\"Visions\"					        DataRef=\"DOSets\\sVI.dat\"/>"
            + "<Set ShortName=\"WL\"    LongName=\"Weatherlight\"            DataRef=\"DOSets\\sWL.dat\"/>"
            + "<Set ShortName=\"VAN\"   LongName=\"Vanguard\"				        DataRef=\"DOSets\\sVAN.dat\"/>"
            + "<Set ShortName=\"PRM\"   LongName=\"Promotional\"		          DataRef=\"DOSets\\sPRM.dat\"/>"
            + "<Set ShortName=\"1E\"    LongName=\"Alpha\"					          DataRef=\"DOSets\\s1E.dat\"/>"
            + "<Set ShortName=\"2E\"    LongName=\"Beta\"					          DataRef=\"DOSets\\s2E.dat\"/>"
            + "<Set ShortName=\"2U\"    LongName=\"Unlimited\"				        DataRef=\"DOSets\\s2U.dat\"/>"
            + "<Set ShortName=\"3E\"    LongName=\"Revised Edition\"		      DataRef=\"DOSets\\s3E.dat\"/>"
            + "<Set ShortName=\"4E\"    LongName=\"Fourth Edition\"		      DataRef=\"DOSets\\s4E.dat\"/>"
            + "<Set ShortName=\"CH\"    LongName=\"Chronicles\"				      DataRef=\"DOSets\\sCH.dat\"/>"
            + "<Set ShortName=\"5E\"    LongName=\"Fifth Edition\"			      DataRef=\"DOSets\\s5E.dat\"/>"
            + "<Set ShortName=\"6E\"    LongName=\"Classic (Sixth Edition)\" DataRef=\"DOSets\\s6E.dat\"/>"
            + "<Set ShortName=\"PO\"    LongName=\"Portal\"					        DataRef=\"DOSets\\sPO.dat\"/>"
            + "<Set ShortName=\"P2\"    LongName=\"Portal Second Age\"	      DataRef=\"DOSets\\sP2.dat\"/>"
            + "<Set ShortName=\"PK\"    LongName=\"Portal Three Kingdoms\"	  DataRef=\"DOSets\\sPK.dat\"/>"
            + "<Set ShortName=\"AN\"    LongName=\"Arabian Nights\"		      DataRef=\"DOSets\\sAN.dat\"/>"
            + "<Set ShortName=\"AQ\"    LongName=\"Antiquities\"				      DataRef=\"DOSets\\sAQ.dat\"/>"
            + "<Set ShortName=\"LE\"    LongName=\"Legends\"					        DataRef=\"DOSets\\sLE.dat\"/>"
            + "<Set ShortName=\"DK\"    LongName=\"The Dark\"				        DataRef=\"DOSets\\sDK.dat\"/>"
            + "<Set ShortName=\"FE\"    LongName=\"Fallen Empires\"		      DataRef=\"DOSets\\sFE.dat\"/>"
            + "<Set ShortName=\"IA\"    LongName=\"Ice Age\"					        DataRef=\"DOSets\\sIA.dat\"/>"
            + "<Set ShortName=\"HM\"    LongName=\"Homelands\"				        DataRef=\"DOSets\\sHM.dat\"/>"
            + "<Set ShortName=\"AL\"    LongName=\"Alliances\"				        DataRef=\"DOSets\\sAL.dat\"/>"
            + "<Set ShortName=\"UZ\"    LongName=\"Urza's Saga\"			        DataRef=\"DOSets\\sUZ.dat\"/>"
            + "<Set ShortName=\"UL\"    LongName=\"Urza's Legacy\"		        DataRef=\"DOSets\\sUL.dat\"/>"
            + "<Set ShortName=\"UD\"    LongName=\"Urza's Destiny\"	        DataRef=\"DOSets\\sUD.dat\"/>"
            + "<Set ShortName=\"MM\"    LongName=\"Mercadian Masques\"	      DataRef=\"DOSets\\sMM.dat\"/>"
            + "<Set ShortName=\"NE\"    LongName=\"Nemesis\"					        DataRef=\"DOSets\\sNE.dat\"/>"
            + "<Set ShortName=\"PR\"    LongName=\"Prophecy\"				        DataRef=\"DOSets\\sPR.dat\"/>"
            + "</Sets>";

        [Test]
        public void LoadSetsViaXml()
        {
           MagicOnlineBotDb dataContext = new MagicOnlineBotDb(ConfigurationManager.ConnectionStrings["CardDataLayer.Properties.Settings.MagicOnlineBotConnectionString"].ToString());

            XDocument loadedSets = XDocument.Load(new StringReader(sets));
            var cardSets = from c in loadedSets.Descendants("Set")
                    select new
                               {
                                   Name = c.Attribute("LongName").Value,
                                   Set = c.Attribute("ShortName").Value
                               };

            foreach (var set in cardSets)
            {
                var cardSet = new CardSet();
                cardSet.CardSetId = 0;
                cardSet.CardSetName = set.Name;
                cardSet.CardSetAbrv = set.Set;
                dataContext.CardSets.InsertOnSubmit(cardSet);
            }

            dataContext.SubmitChanges();
        }
    }
}