﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal class ReplayManager
  {
    public async Task<Replay> GetReplay(string id)
    {
      await Task.Delay(1);
      if (null != id && Replays.ContainsKey(id))
      {
        return Replay.Parse(Replays[id]);
      }
      return null;
    }

    private static readonly Dictionary<string, string> Replays = new Dictionary<string, string>
    {
      {"2017043022gm-0089-0000-c47632a3", Data},
      {"2017042905gm-0009-7447-0f7d7d36", AddedKanData},
      {"2017042917gm-0089-0000-62dfb255", ClosedAndCalledKan}
    };

    // http://tenhou.net/0/?log=2017043022gm-0089-0000-c47632a3&tw=0&ts=5
    private const string Data =
      "<mjloggm ver=\"2.3\">" +
      "<SHUFFLE seed=\"mt19937ar-sha512-n288-base64,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\" ref=\"\"/>" +
      "<GO type=\"137\" lobby=\"0\"/>" +
      "<UN n0=\"%E6%9D%B1%E9%A2%A8%E8%8D%98%E7%AC%AC%32%E2%98%85%E3%83%A9%E3%82%AF\" n1=\"%73%70%69%6E%65%73\" n2=\"%E6%96%AF%E3%81%8F%E6%96%AF%E3%81%8F%E7%84%B6%E3%80%85\" n3=\"%6F%67%61%77%61%6D\" dan=\"11,13,12,12\" rate=\"1539.81,1716.64,1863.54,1706.68\" sx=\"M,M,M,F\"/>" +
      "<TAIKYOKU oya=\"0\"/>" +
      "<INIT seed=\"0,0,0,3,5,31\" ten=\"250,250,250,250\" oya=\"0\" hai0=\"10,19,101,121,132,70,118,11,18,5,93,98,106\" hai1=\"80,7,63,112,130,35,92,122,81,12,113,62,123\" hai2=\"94,50,33,84,74,16,2,110,4,115,30,65,28\" hai3=\"29,22,105,129,37,133,17,89,8,52,83,24,56\"/>" +
      "<T6/><D121/><N who=\"1\" m=\"46091\" /><E12/><V21/><F74/><W71/><G37/><T45/><D132/><U13/><E13/><V124/><F115/><N who=\"1\" m=\"44105\" /><E7/><V75/><F75/><W108/><G71/><T131/><D131/><U85/><E130/><V3/><F65/><W60/><G129/><T26/><D118/><U57/><E92/><V96/><F124/><W126/><G105/><T0/><D70/><U116/><E85/><V102/><F110/><W125/><G133/><T100/><D45/><U1/><E57/><V86/><F4/><W64/><G108/><T36/><D36/><U20/><E20/><V55/><F2/><W127/><G8/><T48/><D48/><U44/><E44/><V73/><F73/><W91/><G83/><N who=\"1\" m=\"31818\" /><E1/><V79/><F3/><W34/><G64/><T14/><D106/><U42/><E42/><V87/><F79/><W15/>" +
      "<AGARI ba=\"0,0\" hai=\"15,17,22,24,29,34,52,56,60,89,91,125,126,127\" machi=\"15\" ten=\"30,7900,0\" yaku=\"0,1,18,1,52,1,54,1\" doraHai=\"31\" who=\"3\" fromWho=\"3\" sc=\"250,-39,250,-20,250,-20,250,79\" />" +
      "<INIT seed=\"1,0,0,2,4,69\" ten=\"211,230,230,329\" oya=\"1\" hai0=\"95,88,90,57,82,93,83,56,130,59,63,54,1\" hai1=\"131,41,118,33,77,110,122,116,28,117,89,60,96\" hai2=\"123,81,134,128,27,61,22,92,34,64,106,98,12\" hai3=\"91,65,40,119,80,97,14,84,46,10,78,115,7\"/>" +
      "<U108/><E41/><V4/><F123/><W101/><G115/><T113/><D130/><U112/><E60/><V120/><F120/><W3/><G65/><T24/><D113/><U85/><E28/><V38/><F106/><W72/><G119/><T44/><D1/><U15/><E15/><V17/><F81/><N who=\"0\" m=\"30730\" /><D24/><U99/><E33/><V86/><F86/><W129/><G129/><T5/><D5/><U121/><E131/><V11/><F38/><W126/><G126/><T107/><D107/><U43/><E43/><V53/><F53/><W114/><G114/><T103/><D103/><U18/><E18/><V58/><F128/><W55/><G55/><T13/><D13/><U36/><E36/><V2/><F134/><W35/><G35/><T125/><D125/><U31/><E31/><V111/><F111/><N who=\"1\" m=\"42537\" /><E112/><V124/><F124/><W32/><G32/><T70/><D70/><U67/><E67/><V39/><F39/><W25/><G25/><T45/><D54/><U74/><E74/><V52/><F34/><W102/><G14/><T73/><D63/><U66/><E66/><V75/><F75/><W62/><G62/><T135/><D73/><U71/><E71/><V104/><F104/><W37/><G97/><N who=\"1\" m=\"37450\" /><E77/><V87/><F64/><W29/><G29/><T49/><D49/><U6/><E6/><V51/><F61/><W132/><G132/><T133/><D95/>" +
      "<AGARI ba=\"0,0\" hai=\"85,89,95,116,117,118,121,122\" m=\"37450,42537\" machi=\"95\" ten=\"40,12000,1\" yaku=\"14,1,10,1,34,2\" doraHai=\"69\" who=\"1\" fromWho=\"0\" sc=\"211,-120,230,120,230,0,329,0\" />" +
      "<AGARI ba=\"0,0\" hai=\"3,7,10,37,40,46,72,78,80,84,91,95,101,102\" machi=\"95\" ten=\"30,7700,0\" yaku=\"7,1,25,2,52,1\" doraHai=\"69\" who=\"3\" fromWho=\"0\" sc=\"91,-77,350,0,230,0,329,77\" />" +
      "<INIT seed=\"1,1,0,4,4,23\" ten=\"14,350,230,406\" oya=\"1\" hai0=\"89,98,21,55,84,93,57,9,131,129,78,132,134\" hai1=\"10,114,17,109,58,70,14,24,40,113,16,3,83\" hai2=\"54,124,49,30,8,26,71,65,12,33,0,94,106\" hai3=\"7,18,72,135,96,46,31,19,120,125,102,99,13\"/>" +
      "<U80/><E70/><V20/><F106/><W48/><G120/><T15/><D78/><U69/><E69/><V60/><F124/><W121/><G121/><T73/><D73/><U64/><E40/><V34/><F0/><W63/><G72/><T123/><D123/><U126/><E126/><V43/><F43/><W122/><G122/><T118/><D118/><U77/><E3/><V6/><F94/><W133/><G125/><T81/><D131/><U116/><E116/><V110/><F110/><W53/><G63/><T32/><D32/><U11/><E109/><V47/>" +
      "<AGARI ba=\"1,0\" hai=\"6,8,12,20,26,30,33,34,47,49,54,60,65,71\" machi=\"47\" ten=\"20,2700,0\" yaku=\"0,1,7,1,52,1\" doraHai=\"23\" who=\"2\" fromWho=\"2\" sc=\"14,-8,350,-14,230,30,406,-8\" />" +
      "<INIT seed=\"2,0,0,2,2,26\" ten=\"6,336,260,398\" oya=\"2\" hai0=\"19,109,42,0,57,123,58,34,86,101,14,27,3\" hai1=\"35,106,129,78,41,17,43,38,97,131,107,128,21\" hai2=\"79,18,66,31,71,85,45,9,7,36,6,4,93\" hai3=\"108,89,12,28,103,104,47,33,102,133,115,126,13\"/>" +
      "<V81/><F36/><W25/><G108/><T99/><D109/><U98/><E78/><V94/><F71/><W114/><G126/><T96/><D123/><U82/><E82/><V120/><F120/><W59/><G133/><T5/><D34/><U32/><E38/><V64/><F9/><W92/><G47/><T52/><D42/><N who=\"1\" m=\"15883\" /><E21/><V118/><F118/><W113/><G104/><N who=\"1\" m=\"39978\" /><E17/><V53/><F18/><W29/><G59/><T46/><D101/><N who=\"3\" m=\"38409\" /><G29/><T37/><D37/><U75/><E75/><V74/><F74/><W44/><G44/><T117/><D117/><U132/><E132/><V16/><F16/><W77/><G77/><T127/><D127/><U73/><E73/><V80/><F45/><W62/><G62/><N who=\"0\" m=\"36135\" /><D86/>" +
      "<AGARI ba=\"0,0\" hai=\"12,13,25,28,33,86,89,92,113,114,115\" m=\"38409\" machi=\"86\" ten=\"30,2000,0\" yaku=\"11,1,52,1\" doraHai=\"26\" who=\"3\" fromWho=\"0\" sc=\"6,-20,336,0,260,0,398,20\" owari=\"-14,-51.0,336,14.0,260,-14.0,418,51.0\" />" +
      "</mjloggm>";

    private const string AddedKanData =
        "<mjloggm ver=\"2.3\"><SHUFFLE seed=\"mt19937ar-sha512-n288-base64,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\" ref=\"\"/><GO type=\"9\" lobby=\"7447\"/><UN n0=\"%4F%65%53%69%6C%66%72%6F\" n1=\"%41%69%6E%68%61%72%74\" n2=\"%57%61%6E%65%77\" n3=\"%73%70%69%6E%65%73\" dan=\"8,0,0,13\" rate=\"1483.89,1470.07,1460.54,1717.37\" sx=\"F,M,F,M\"/><TAIKYOKU oya=\"0\"/><INIT seed=\"0,0,0,4,2,121\" ten=\"250,250,250,250\" oya=\"0\" hai0=\"111,124,33,116,16,101,55,45,54,98,65,93,75\" hai1=\"71,72,25,80,104,108,78,41,13,74,102,19,30\" hai2=\"26,37,28,135,89,118,12,18,130,49,77,34,76\" hai3=\"106,32,42,56,99,94,112,3,63,21,60,67,83\"/><T64/><D75/><U24/><E108/><V134/><F130/><W51/><G112/><T115/><D115/><U66/><E41/><V132/><F89/><W81/><G32/><T1/><D1/><U131/><E131/><V52/><F37/><W9/><G106/><T50/><D116/><U58/><E72/><V91/><F91/><W8/><G63/><T129/><D129/><U105/><E102/><V119/><F12/><W14/><G3/><T110/><D124/><U88/><E71/><V61/><F61/><W59/><G59/><T4/><D33/><U86/><E66/><V122/><F122/><W0/><G0/><T90/><D4/><U15/><E58/><V126/><F126/><W11/><G21/><T114/><D114/><U40/><E40/><V120/><F120/><W113/><G113/><T68/><D101/><U127/><E127/><V47/><REACH who=\"2\" step=\"1\"/><F18/><REACH who=\"2\" ten=\"250,250,240,250\" step=\"2\"/><W128/><G128/><T7/><D7/><U69/><E13/><V39/><F39/><W73/><G14/><T85/><D85/><U29/><E15/><V109/><F109/><N who=\"0\" m=\"41482\" /><D68/><U22/><E19/><V2/><F2/><W92/><G73/><T97/><D97/><U100/><E69/><V20/><F20/><W82/><G8/><T17/><D55/><U103/><E88/><V43/><F43/><W38/><G38/><T23/><D17/><U6/><E86/><V10/><F10/><N who=\"3\" m=\"3595\" /><G99/><T44/><D44/><AGARI ba=\"0,1\" hai=\"42,44,51,56,60,67,81,82,83,92,94\" m=\"3595\" machi=\"44\" ten=\"30,1000,0\" yaku=\"8,1\" doraHai=\"121\" who=\"3\" fromWho=\"0\" sc=\"250,-10,250,0,240,0,250,20\" /><INIT seed=\"1,0,0,5,2,88\" ten=\"240,250,240,270\" oya=\"1\" hai0=\"25,120,16,79,33,47,1,71,94,19,101,46,10\" hai1=\"129,21,65,82,59,13,85,112,97,73,113,84,81\" hai2=\"103,60,51,92,83,127,123,130,38,99,90,20,102\" hai3=\"50,86,110,106,122,134,55,39,45,40,18,30,63\"/><U3/><E3/><V35/><F38/><W104/><G110/><T70/><D79/><U17/><E129/><V42/><F123/><W66/><G134/><T6/><D120/><U7/><E7/><V121/><F121/><W22/><G122/><T68/><D33/><U2/><E2/><V96/><F83/><W67/><G86/><T64/><D64/><U52/><E73/><V43/><F60/><W126/><G67/><T34/><D34/><U69/><E97/><V124/><F35/><W95/><G30/><T54/><D54/><U14/><E69/><V15/><F130/><W72/><G72/><T77/><D77/><N who=\"1\" m=\"46127\" /><E65/><V11/><F20/><W75/><G75/><T27/><D27/><N who=\"1\" m=\"14767\" /><E113/><V111/><F111/><W108/><G108/><T53/><D53/><U32/><E32/><V9/><F90/><W36/><G106/><T8/><D25/><U131/><E131/><V24/><F24/><W125/><G104/><T23/><D10/><U132/><E132/><V29/><F29/><W28/><G28/><T119/><D119/><U118/><E118/><V87/><F87/><W26/><G39/><T117/><D117/><U109/><E109/><V114/><F114/><W91/><G63/><T100/><D100/><U4/><E4/><V57/><F57/><W41/><G36/><T74/><D74/><U12/><E112/><V98/><F98/><N who=\"3\" m=\"57727\" /><G66/><T76/><D76/><N who=\"1\" m=\"46279\" /><E59/><V107/><F51/><W61/><G61/><T31/><D31/><U48/><E48/><V80/><F80/><W0/><G0/><T37/><D37/><U58/><E58/><V116/><F116/><RYUUKYOKU ba=\"0,0\" sc=\"240,-15,250,15,240,-15,270,15\" hai1=\"12,13,14,52\" hai3=\"18,22,26,40,41,45,50,55,125,126\" /><INIT seed=\"1,1,0,3,3,45\" ten=\"225,265,225,285\" oya=\"1\" hai0=\"129,1,112,52,95,51,32,94,132,50,102,60,126\" hai1=\"71,111,6,12,65,14,125,49,24,106,62,23,56\" hai2=\"130,0,43,13,31,93,115,53,76,120,110,128,26\" hai3=\"64,17,9,91,74,22,72,30,58,7,97,55,92\"/><U8/><E111/><V36/><F53/><W2/><G64/><T20/><D112/><U42/><E125/><V122/><F110/><W127/><G30/><T63/><D126/><U16/><E106/><V83/><F93/><W68/><G68/><T114/><D129/><N who=\"2\" m=\"49770\" /><F0/><W33/><G33/><T57/><D114/><U107/><E107/><V40/><F36/><W10/><G10/><T54/><D132/><U104/><E104/><V41/><F13/><W44/><G44/><T99/><D1/><U118/><E118/><V98/><F98/><W90/><G90/><T119/><D119/><U109/><E109/><V117/><F117/><W48/><REACH who=\"3\" step=\"1\"/><G127/><REACH who=\"3\" ten=\"225,265,225,275\" step=\"2\"/><T38/><D32/><U134/><E134/><V70/><F70/><W18/><G18/><T34/><D34/><U79/><E71/><V89/><F115/><W46/><G46/><T78/><D38/><U96/><E79/><V116/><F116/><W86/><G86/><T87/><D20/><U3/><E3/><V39/><F39/><W88/><G88/><N who=\"0\" m=\"53535\" /><D78/><U61/><E96/><V47/><F47/><AGARI ba=\"1,1\" hai=\"47,50,51,52,54,57,60,63,95,99,102\" m=\"53535\" machi=\"47\" ten=\"30,8000,1\" yaku=\"8,1,52,2,54,2\" doraHai=\"45\" who=\"0\" fromWho=\"2\" sc=\"225,93,265,0,225,-83,275,0\" /><INIT seed=\"2,0,0,3,4,21\" ten=\"318,265,142,275\" oya=\"2\" hai0=\"60,52,115,59,118,99,1,127,70,9,44,120,116\" hai1=\"93,67,22,109,64,45,86,10,117,80,15,16,74\" hai2=\"5,11,71,40,125,72,47,78,57,129,4,95,61\" hai3=\"62,132,3,29,18,106,14,77,133,103,43,131,81\"/><V7/><F71/><W75/><G29/><T96/><D115/><U124/><E109/><V32/><F95/><W55/><G131/><T56/><D120/><U42/><E117/><N who=\"0\" m=\"45161\" /><D1/><U34/><E34/><V82/><F125/><W85/><G43/><T88/><D9/><U41/><E124/><V20/><F32/><W69/><G3/><T134/><D127/><U114/><E114/><V65/><F129/><W98/><G85/><T113/><D113/><U23/><E74/><V122/><F122/><W91/><G91/><T6/><D6/><N who=\"1\" m=\"3543\" /><E93/><V26/><F26/><W58/><REACH who=\"3\" step=\"1\"/><G69/><REACH who=\"3\" ten=\"318,265,142,265\" step=\"2\"/><T53/><D134/><U54/><E41/><V83/><F83/><W89/><G89/><T38/><D70/><U0/><E0/><N who=\"2\" m=\"391\" /><F20/><AGARI ba=\"0,1\" hai=\"14,18,20,55,58,62,75,77,81,98,103,106,132,133\" machi=\"20\" ten=\"40,1300,0\" yaku=\"1,1,53,0\" doraHai=\"21\" doraHaiUra=\"2\" who=\"3\" fromWho=\"2\" sc=\"318,0,265,0,142,-13,265,23\" /><INIT seed=\"3,0,0,5,0,14\" ten=\"318,265,129,288\" oya=\"3\" hai0=\"42,62,95,74,29,25,8,2,75,120,114,59,70\" hai1=\"7,31,129,130,13,92,4,11,61,35,38,41,44\" hai2=\"80,52,105,117,63,122,87,132,100,60,30,123,39\" hai3=\"102,108,19,104,79,99,53,58,72,73,1,115,47\"/><W77/><G1/><T48/><D120/><N who=\"2\" m=\"46122\" /><F117/><W5/><G115/><T43/><D114/><U106/><E106/><V135/><F30/><W125/><G125/><T112/><D95/><U68/><E92/><V57/><F39/><W103/><G103/><T50/><D112/><U51/><E4/><V33/><F33/><W119/><G119/><T107/><D107/><U94/><E94/><V69/><F69/><W6/><G47/><T113/><D113/><U83/><E83/><V12/><F12/><W131/><G131/><N who=\"1\" m=\"50186\" /><E31/><V17/><F17/><W71/><G71/><T128/><D128/><U89/><E35/><V78/><F105/><W86/><G108/><T65/><D2/><U21/><E89/><V121/><F121/><W10/><G73/><T96/><D96/><U126/><E126/><V84/><F84/><W67/><G67/><T101/><D101/><U81/><E81/><V3/><F3/><W36/><G72/><T110/><D110/><U55/><E21/><V133/><F100/><W20/><G36/><T66/><D8/><U16/><E68/><V64/><F64/><W116/><G10/><T24/><D66/><U91/><E91/><V45/><F45/><W18/><G116/><T76/><D70/><U56/><E7/><V111/><F111/><W22/><G86/><T0/><D0/><U26/><E26/><N who=\"0\" m=\"10345\" /><D29/><U85/><E85/><V23/><F23/><N who=\"3\" m=\"8747\" /><G5/><T27/><N who=\"0\" m=\"10353\" /><T34/><DORA hai=\"124\" /><D34/><U97/><E97/><V37/><F37/><AGARI ba=\"0,0\" hai=\"11,13,16,37,38,41,44,51,55,56,61\" m=\"50186\" machi=\"37\" ten=\"30,12000,2\" yaku=\"19,1,52,4,54,1\" doraHai=\"14,124\" who=\"1\" fromWho=\"2\" sc=\"318,0,265,120,129,-120,288,0\" /><INIT seed=\"4,0,0,1,4,75\" ten=\"318,385,9,288\" oya=\"0\" hai0=\"84,18,100,92,121,17,96,62,112,128,119,60,85\" hai1=\"63,125,90,113,83,48,8,25,42,80,108,45,15\" hai2=\"82,118,76,57,7,78,46,105,50,10,97,11,120\" hai3=\"24,91,87,73,23,98,110,72,36,29,131,41,2\"/><T81/><D119/><U13/><E108/><V65/><F105/><W127/><G110/><T30/><D121/><U19/><E125/><V64/><F97/><W95/><G127/><T114/><D30/><U26/><E113/><N who=\"0\" m=\"43625\" /><D128/><U1/><E1/><V14/><F118/><W28/><G131/><T32/><D81/><N who=\"1\" m=\"31307\" /><E63/><N who=\"0\" m=\"24105\" /><D32/><U106/><E106/><V86/><F76/><W133/><G2/><T40/><D40/><U117/><E117/><V68/><F68/><W55/><G133/><T88/><D100/><U93/><E13/><V34/><F34/><W0/><G0/><T109/><D109/><U134/><E134/><V99/><F99/><AGARI ba=\"0,0\" hai=\"8,15,19,25,26,42,45,48,90,93,99\" m=\"31307\" machi=\"99\" ten=\"30,1000,0\" yaku=\"8,1\" doraHai=\"75\" who=\"1\" fromWho=\"2\" sc=\"318,0,385,10,9,-10,288,0\" owari=\"318,12.0,395,49.0,-1,-50.0,288,-11.0\" /></mjloggm>";

    private const string ClosedAndCalledKan =
        "<mjloggm ver=\"2.3\"><SHUFFLE seed=\"mt19937ar-sha512-n288-base64,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\" ref=\"\"/><GO type=\"137\" lobby=\"0\"/><UN n0=\"%73%70%69%6E%65%73\" n1=\"%6E%6F%62%33%33\" n2=\"%6B%65%6E%34%36\" n3=\"%6D%61%73%79%75%32%38\" dan=\"13,11,11,13\" rate=\"1717.37,1417.07,1571.42,1761.44\" sx=\"M,M,M,M\"/><TAIKYOKU oya=\"0\"/><INIT seed=\"0,0,0,3,4,35\" ten=\"250,250,250,250\" oya=\"0\" hai0=\"18,45,60,5,52,2,54,82,105,97,17,89,66\" hai1=\"44,88,37,3,57,24,55,119,132,27,98,114,34\" hai2=\"12,111,62,48,28,42,41,95,10,29,65,78,73\" hai3=\"36,94,91,108,30,46,19,13,79,116,117,4,70\"/><T71/><D54/><U102/><E119/><V90/><F111/><W130/><G70/><T106/><D106/><U25/><E114/><V128/><F128/><W87/><G108/><T75/><D105/><U80/><E37/><V99/><F73/><W124/><G130/><T122/><D122/><U84/><E34/><V38/><F38/><W50/><G36/><T77/><D97/><U92/><E44/><V133/><F78/><W104/><G104/><T76/><D76/><U23/><E132/><V83/><F133/><W86/><G124/><T120/><D89/><U26/><E3/><V103/><F42/><W32/><G32/><T135/><D135/><U31/><E27/><V129/><F129/><W109/><G30/><T125/><D120/><U14/><E14/><V115/><F115/><W113/><G109/><T121/><D125/><U96/><E98/><V15/><F15/><W64/><G64/><T8/><REACH who=\"0\" step=\"1\"/><D121/><REACH who=\"0\" ten=\"240,250,250,250\" step=\"2\"/><U16/><REACH who=\"1\" step=\"1\"/><E31/><REACH who=\"1\" ten=\"240,240,250,250\" step=\"2\"/><V11/><F29/><W1/><G113/><T67/><D67/><U9/><E9/><V72/><F10/><W134/><G134/><T22/><D22/><U81/><E81/><V118/><F83/><W39/><G117/><T7/><D7/><U43/><E43/><V58/><F41/><W85/><G116/><T112/><D112/><U56/><E56/><V131/><F28/><W127/><G4/><T51/><AGARI ba=\"0,2\" hai=\"2,5,8,17,18,45,51,52,60,66,71,75,77,82\" machi=\"51\" ten=\"30,12000,1\" yaku=\"1,1,0,1,52,1,54,1,53,1\" doraHai=\"35\" doraHaiUra=\"47\" who=\"0\" fromWho=\"0\" sc=\"240,140,240,-40,250,-40,250,-40\" /><INIT seed=\"0,1,0,4,1,6\" ten=\"380,200,210,210\" oya=\"0\" hai0=\"105,62,124,2,68,106,66,3,109,60,103,99,13\" hai1=\"88,113,11,75,85,9,107,126,83,33,116,16,117\" hai2=\"130,95,36,132,84,31,129,14,37,67,90,92,114\" hai3=\"26,0,87,73,42,34,65,70,22,82,78,21,55\"/><T49/><D124/><U72/><E33/><V46/><F114/><W58/><G0/><T89/><D106/><U100/><E126/><V10/><F132/><W76/><G70/><T97/><D60/><U77/><E113/><V23/><F67/><W59/><G34/><T135/><D135/><U12/><E75/><V38/><F95/><W39/><G39/><T57/><D109/><U61/><E61/><V110/><F110/><W93/><G42/><T127/><D127/><U81/><E11/><V4/><F31/><W86/><G73/><T112/><D112/><U123/><E123/><V43/><REACH who=\"2\" step=\"1\"/><F23/><REACH who=\"2\" ten=\"380,200,200,210\" step=\"2\"/><W98/><G65/><T44/><D68/><U18/><E18/><V119/><F119/><W111/><G111/><T74/><D66/><U101/><E117/><V91/><F91/><W48/><G21/><T80/><D89/><U47/><E116/><V64/><F64/><W104/><G104/><T41/><D105/><U79/><E101/><V45/><F45/><W28/><G76/><T131/><D103/><U24/><E79/><V54/><F54/><W17/><G28/><T102/><D102/><U25/><E47/><V108/><F108/><W29/><G29/><T121/><D44/><U56/><E56/><V125/><F125/><W40/><G59/><T94/><D57/><U96/><AGARI ba=\"1,1\" hai=\"9,12,16,24,25,72,77,81,83,85,88,96,100,107\" machi=\"96\" ten=\"30,7900,0\" yaku=\"0,1,52,1,54,2\" doraHai=\"6\" who=\"1\" fromWho=\"1\" sc=\"380,-40,200,92,200,-21,210,-21\" /><INIT seed=\"1,0,0,1,2,135\" ten=\"340,292,179,189\" oya=\"1\" hai0=\"15,1,61,9,114,25,24,51,20,71,64,120,16\" hai1=\"105,4,133,46,47,79,112,113,115,127,73,121,72\" hai2=\"60,31,41,134,66,129,38,80,110,48,19,96,63\" hai3=\"62,11,5,14,103,83,125,10,68,59,7,87,42\"/><U91/><E121/><V8/><F134/><W26/><G68/><T49/><D120/><U82/><E133/><V122/><F122/><W43/><G103/><T12/><D114/><U130/><E105/><V118/><F118/><W126/><G26/><T6/><D15/><U84/><E4/><V88/><F38/><W65/><G83/><T85/><D85/><U75/><E47/><V28/><F110/><W104/><G104/><T36/><D36/><U93/><E46/><V57/><F129/><W94/><G94/><T37/><D37/><U102/><E102/><V70/><F41/><W17/><G87/><T117/><D117/><U45/><E45/><V35/><F48/><W53/><G65/><T18/><REACH who=\"0\" step=\"1\"/><D25/><REACH who=\"0\" ten=\"330,292,179,189\" step=\"2\"/><U29/><E29/><V106/><F31/><W23/><G5/><T100/><D100/><U0/><E0/><V52/><F28/><W81/><G81/><T40/><D40/><U22/><E130/><V34/><F80/><W56/><G7/><T119/><D119/><U55/><E55/><V98/><F35/><W99/><G99/><T32/><D32/><U128/><E128/><V76/><F34/><W132/><G132/><T111/><D111/><U74/><E22/><AGARI ba=\"0,1\" hai=\"1,6,9,12,16,18,20,22,24,49,51,61,64,71\" machi=\"22\" ten=\"30,3900,0\" yaku=\"1,1,7,1,54,1,53,0\" doraHai=\"135\" doraHaiUra=\"108\" who=\"0\" fromWho=\"1\" sc=\"330,49,292,-39,179,0,189,0\" /><INIT seed=\"2,0,0,2,4,135\" ten=\"379,253,179,189\" oya=\"2\" hai0=\"115,13,55,1,42,11,82,49,53,97,35,61,10\" hai1=\"134,19,98,69,44,60,84,24,14,28,48,129,121\" hai2=\"103,88,124,74,26,39,118,116,30,0,110,27,7\" hai3=\"45,120,41,57,15,91,92,52,77,78,99,25,16\"/><V119/><F39/><W18/><G120/><T58/><D115/><U43/><E121/><V22/><F74/><W111/><G111/><T67/><D1/><U17/><E129/><V80/><F103/><W33/><G33/><T50/><D35/><U71/><E134/><V29/><F80/><W133/><G133/><T122/><D122/><U89/><E98/><V107/><F107/><W64/><G64/><T131/><D131/><U90/><E90/><V105/><F105/><W106/><G106/><T4/><D11/><U46/><E46/><V56/><F56/><W72/><G72/><T63/><D97/><U62/><E28/><V8/><F88/><W75/><G75/><T73/><D42/><U85/><E24/><V3/><F3/><W95/><G95/><T125/><D73/><U68/><E85/><V37/><F37/><W108/><G108/><T38/><D82/><U123/><E123/><V54/><F54/><N who=\"0\" m=\"20490\" /><D63/><U86/><E86/><V36/><F36/><W127/><G127/><T83/><D125/><U12/><E84/><V112/><F112/><W6/><G6/><T128/><D38/><U23/><E89/><V132/><F110/><W31/><G31/><T114/><D128/><U126/><REACH who=\"1\" step=\"1\"/><E126/><REACH who=\"1\" ten=\"379,243,179,189\" step=\"2\"/><V104/><F124/><W59/><G45/><T34/><D114/><U96/><E96/><V5/><F132/><W100/><G100/><T20/><D83/><U76/><E76/><V2/><F2/><W66/><G78/><T21/><D34/><U113/><E113/><V65/><F30/><W9/><G77/><T32/><D32/><U47/><E47/><RYUUKYOKU ba=\"0,1\" sc=\"379,15,243,15,179,-15,189,-15\" hai0=\"4,10,13,20,21,49,50,58,61,67\" hai1=\"12,14,17,19,23,43,44,48,60,62,68,69,71\" /><INIT seed=\"3,1,1,5,0,130\" ten=\"394,258,164,174\" oya=\"3\" hai0=\"3,56,2,12,40,68,11,51,43,23,131,59,123\" hai1=\"15,128,75,99,90,122,45,113,83,26,86,57,33\" hai2=\"74,65,100,58,129,120,135,38,96,34,44,111,105\" hai3=\"110,73,66,27,97,62,102,21,64,82,72,17,41\"/><W53/><G110/><T109/><D109/><U91/><E113/><V54/><F111/><W114/><G114/><T103/><D131/><U8/><E122/><V71/><F129/><W25/><G41/><T42/><D123/><U78/><E128/><V76/><F120/><W6/><G6/><T124/><D68/><U127/><E127/><V63/><F74/><W69/><G66/><T119/><D124/><U132/><E33/><V95/><F34/><W93/><G53/><T10/><D119/><U52/><E26/><V28/><F76/><W9/><G27/><T107/><D23/><U116/><E116/><V101/><F28/><W89/><G9/><T87/><D87/><U106/><E45/><V29/><F29/><W49/><G49/><T48/><D12/><U39/><E39/><V13/><F105/><W112/><G112/><T20/><D20/><U115/><E115/><V14/><F101/><W1/><G1/><T84/><D84/><U81/><E91/><V85/><F85/><W55/><G55/><T79/><D43/><U4/><E132/><V121/><F135/><W50/><G50/><T18/><D107/><U47/><E47/><V5/><F5/><W31/><G82/><T19/><D103/><U7/><E7/><V104/><F104/><W30/><G102/><T125/><D125/><U37/><E37/><V98/><F98/><W24/><G17/><T32/><D32/><AGARI ba=\"1,1\" hai=\"21,24,25,30,31,32,62,64,69,72,73,89,93,97\" machi=\"32\" ten=\"30,1500,0\" yaku=\"7,1\" doraHai=\"130\" who=\"3\" fromWho=\"0\" sc=\"394,-18,258,0,164,0,174,28\" /><INIT seed=\"3,2,0,4,2,0\" ten=\"376,258,164,202\" oya=\"3\" hai0=\"19,46,118,4,75,33,35,130,71,125,110,53,16\" hai1=\"3,52,38,73,36,94,50,57,126,34,114,68,63\" hai2=\"81,17,99,47,115,80,76,18,86,84,85,87,42\" hai3=\"44,74,37,25,93,131,98,78,66,129,11,135,100\"/><W133/><G66/><T48/><D125/><U108/><E73/><V117/><F117/><W116/><G25/><T9/><D130/><N who=\"3\" m=\"49673\" /><G11/><T5/><D110/><U123/><E34/><V22/><F115/><W2/><G2/><T40/><D118/><U111/><E3/><V82/><F99/><W39/><G116/><T60/><D75/><U54/><E123/><V10/><F76/><W70/><G70/><T102/><D102/><U109/><E94/><V43/><F10/><W21/><G21/><T89/><D19/><U24/><E24/><V65/><F65/><W97/><G97/><T72/><D72/><U69/><E114/><V14/><N who=\"2\" m=\"22016\" /><DORA hai=\"112\" /><V113/><F113/><W51/><G78/><T77/><D89/><U23/><E23/><V29/><F14/><W96/><G96/><T27/><D77/><U62/><E126/><V91/><F91/><W127/><G74/><T64/><D9/><U120/><E120/><V79/><F79/><W121/><G121/><T55/><D35/><U132/><E132/><N who=\"3\" m=\"50762\" /><G127/><T8/><D8/><U105/><E105/><V107/><F107/><W13/><G13/><T31/><REACH who=\"0\" step=\"1\"/><D16/><REACH who=\"0\" ten=\"366,258,164,202\" step=\"2\"/><N who=\"2\" m=\"6250\" /><F47/><W41/><AGARI ba=\"2,1\" hai=\"37,39,41,44,51,93,98,100\" m=\"50762,49673\" machi=\"41\" ten=\"30,3000,0\" yaku=\"19,1,20,1\" doraHai=\"0,112\" who=\"3\" fromWho=\"3\" sc=\"366,-12,258,-12,164,-12,202,46\" /><INIT seed=\"3,3,0,3,4,44\" ten=\"354,246,152,248\" oya=\"3\" hai0=\"34,121,50,21,15,46,35,24,89,73,119,12,120\" hai1=\"78,118,45,52,37,47,122,0,96,110,25,5,86\" hai2=\"106,40,26,117,58,95,57,101,36,129,115,4,82\" hai3=\"105,103,116,65,13,53,97,88,19,74,76,126,62\"/><W41/><G116/><T6/><D119/><U130/><BYE who=\"2\" /><E118/><V75/><F75/><W68/><G126/><T38/><D38/><U30/><E122/><V42/><F42/><W123/><G123/><T85/><D73/><U125/><E125/><V63/><F63/><W102/><G102/><T54/><D34/><U108/><E130/><V134/><F134/><W71/><G71/><T7/><D35/><U80/><E96/><V112/><F112/><W10/><G41/><T64/><D64/><U20/><E0/><V17/><F17/><W81/><G53/><T39/><D39/><U100/><E100/><V90/><F90/><W49/><G49/><T124/><D124/><U104/><E104/><V22/><F22/><W56/><REACH who=\"3\" step=\"1\"/><G88/><REACH who=\"3\" ten=\"354,246,152,238\" step=\"2\"/><T59/><D121/><U51/><E37/><V135/><F135/><W131/><G131/><T43/><D120/><U72/><E5/><V87/><F87/><W11/><G11/><T16/><D6/><U93/><E72/><V66/><F66/><W109/><G109/><N who=\"1\" m=\"42090\" /><E47/><V18/><F18/><W111/><G111/><T132/><D7/><U69/><E69/><V92/><F92/><AGARI ba=\"3,1\" hai=\"20,25,30,45,51,52,78,80,86,92,93\" m=\"42090\" machi=\"92\" ten=\"30,3900,0\" yaku=\"14,1,52,1,54,1\" doraHai=\"44\" who=\"1\" fromWho=\"2\" sc=\"354,0,246,58,152,-48,238,0\" /><INIT seed=\"4,0,0,1,4,107\" ten=\"354,304,104,238\" oya=\"0\" hai0=\"64,133,47,121,112,24,115,111,17,85,26,97,1\" hai1=\"81,49,130,126,71,117,127,122,2,21,96,101,75\" hai2=\"67,95,48,84,5,38,129,6,7,33,44,125,99\" hai3=\"98,106,135,15,63,36,86,94,10,79,72,34,3\"/><T50/><D121/><U13/><E117/><V56/><F56/><W57/><G135/><T35/><D133/><U4/><E122/><V52/><F52/><W76/><G36/><T109/><D1/><U58/><E71/><V116/><F116/><W25/><G3/><T132/><D132/><U102/><E2/><V77/><F77/><W30/><G106/><T42/><D64/><U73/><E130/><V53/><F53/><W103/><G86/><T39/><D85/><U104/><E102/><V65/><F65/><W87/><G87/><T0/><D0/><U88/><E88/><V41/><F41/><W61/><G61/><T93/><D35/><U114/><E81/><V105/><F105/><W18/><REACH who=\"3\" step=\"1\"/><G72/><REACH who=\"3\" ten=\"354,304,104,228\" step=\"2\"/><N who=\"1\" m=\"27722\" /><E114/><N who=\"0\" m=\"43561\" /><D39/><U20/><E4/><V131/><F131/><W124/><G124/><N who=\"1\" m=\"47658\" /><E13/><V8/><F8/><W54/><AGARI ba=\"0,1\" hai=\"10,15,18,25,30,34,54,57,63,76,79,94,98,103\" machi=\"54\" ten=\"20,2700,0\" yaku=\"1,1,0,1,7,1,53,0\" doraHai=\"107\" doraHaiUra=\"120\" who=\"3\" fromWho=\"3\" sc=\"354,-13,304,-7,104,-7,228,37\" /><INIT seed=\"5,0,0,5,1,86\" ten=\"341,297,97,265\" oya=\"1\" hai0=\"59,72,76,41,117,96,70,130,37,2,13,114,42\" hai1=\"60,101,47,7,116,131,67,119,25,115,10,32,11\" hai2=\"111,103,43,12,99,93,19,55,17,15,39,49,31\" hai3=\"123,105,69,132,135,102,120,38,74,81,0,97,108\"/><U104/><E115/><V5/><F5/><W133/><G69/><T56/><D70/><N who=\"1\" m=\"42343\" /><E47/><V26/><F26/><W126/><G38/><T73/><D117/><N who=\"1\" m=\"45131\" /><E131/><V65/><F65/><W27/><G27/><T106/><D114/><U22/><E7/><V3/><F3/><W46/><G46/><T63/><D42/><U128/><E128/><V94/><F94/><W24/><G24/><T100/><D2/><U82/><E22/><V121/><F121/><N who=\"3\" m=\"46667\" /><G0/><T34/><D34/><U129/><E82/><V8/><F8/><W89/><G108/><T4/><D76/><U92/><E92/><V127/><F127/><W98/><G126/><T122/><D59/><U90/><E90/><V125/><F125/><W91/><G97/><T110/><D110/><U95/><E95/><V44/><F44/><W30/><G30/><T77/><D41/><U20/><E20/><V51/><F51/><W9/><G9/><T61/><D37/><U107/><E107/><V88/><F88/><W54/><G54/><T33/><D130/><U113/><E113/><V66/><F66/><W1/><G1/><T87/><D122/><U78/><E78/><AGARI ba=\"0,0\" hai=\"74,78,81,89,91,98,102,105,132,133,135\" m=\"46667\" machi=\"78\" ten=\"40,8000,1\" yaku=\"20,1,34,2,52,2\" doraHai=\"86\" who=\"3\" fromWho=\"1\" sc=\"341,0,297,-80,97,0,265,80\" /><INIT seed=\"6,0,0,0,2,93\" ten=\"341,217,97,345\" oya=\"2\" hai0=\"107,71,83,22,39,46,129,74,112,23,134,1,130\" hai1=\"76,110,60,31,52,99,36,102,21,35,43,87,3\" hai2=\"132,91,47,123,122,92,88,2,18,98,81,7,44\" hai3=\"53,45,108,113,62,119,115,125,58,106,78,6,105\"/><V124/><F124/><W120/><BYE who=\"1\" /><G78/><N who=\"0\" m=\"44503\" /><D134/><U28/><E28/><V116/><F116/><W86/><G6/><T26/><D112/><U55/><E55/><V9/><F9/><W59/><G120/><T64/><D1/><U121/><E121/><V72/><F72/><W100/><G119/><T128/><D26/><U50/><E50/><V109/><F109/><W66/><G108/><T10/><D10/><U49/><E49/><V111/><F111/><W24/><G125/><T73/><D73/><U42/><E42/><V61/><F61/><W8/><G8/><T57/><D57/><U0/><E0/><V118/><F118/><W29/><G45/><T20/><D107/><U30/><E30/><V79/><F79/><W11/><G11/><T69/><D64/><U32/><E32/><V77/><F77/><W131/><G131/><N who=\"0\" m=\"33539\" /><T97/><DORA hai=\"75\" /><D97/><U103/><E103/><V84/><F84/><W25/><G25/><T17/><D17/><U127/><E127/><V51/><F51/><W89/><G106/><T94/><D94/><U40/><E40/><AGARI ba=\"0,0\" hai=\"20,22,23,39,40,46,69,71\" m=\"33539,44503\" machi=\"40\" ten=\"50,3200,0\" yaku=\"19,1,52,1\" doraHai=\"93,75\" who=\"0\" fromWho=\"1\" sc=\"341,32,217,-32,97,0,345,0\" /><INIT seed=\"7,0,0,3,0,29\" ten=\"373,185,97,345\" oya=\"3\" hai0=\"19,82,74,58,71,98,12,99,41,68,103,92,131\" hai1=\"123,59,65,21,66,1,23,115,102,52,31,37,20\" hai2=\"16,114,105,9,18,3,49,2,97,117,126,57,125\" hai3=\"108,30,109,6,33,0,101,78,95,56,84,63,27\"/><W61/><G78/><T128/><D41/><U116/><E116/><V85/><F85/><W45/><G45/><T35/><D58/><U96/><E96/><V50/><F50/><W89/><G101/><T73/><D99/><U110/><E110/><N who=\"3\" m=\"42602\" /><G56/><T26/><D26/><U112/><E112/><V55/><F55/><W80/><G6/><T100/><D100/><U119/><E119/><V129/><F129/><N who=\"0\" m=\"49738\" /><D35/><U53/><E53/><V62/><F62/><N who=\"3\" m=\"23563\" /><G0/><T88/><D82/><AGARI ba=\"0,0\" hai=\"27,30,33,80,82,84,89,95\" m=\"23563,42602\" machi=\"82\" ten=\"30,2900,0\" yaku=\"10,1,52,1\" doraHai=\"29\" who=\"3\" fromWho=\"0\" sc=\"373,-29,185,0,97,0,345,29\" owari=\"344,14.0,185,-21.0,97,-40.0,374,47.0\" /></mjloggm>";
  }
}