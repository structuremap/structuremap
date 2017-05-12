using System;

namespace StructureMap.Testing.Performance
{
    public static class TypeCatalog
    {
        static TypeCatalog()
        {
            var typesMap = new[]
            {
                Tuple.Create(typeof(IService0), typeof(Service0)),
                Tuple.Create(typeof(IService1), typeof(Service1)),
                Tuple.Create(typeof(IService2), typeof(Service2)),
                Tuple.Create(typeof(IService3), typeof(Service3)),
                Tuple.Create(typeof(IService4), typeof(Service4)),
                Tuple.Create(typeof(IService5), typeof(Service5)),
                Tuple.Create(typeof(IService6), typeof(Service6)),
                Tuple.Create(typeof(IService7), typeof(Service7)),
                Tuple.Create(typeof(IService8), typeof(Service8)),
                Tuple.Create(typeof(IService9), typeof(Service9)),
                Tuple.Create(typeof(IService10), typeof(Service10)),
                Tuple.Create(typeof(IService11), typeof(Service11)),
                Tuple.Create(typeof(IService12), typeof(Service12)),
                Tuple.Create(typeof(IService13), typeof(Service13)),
                Tuple.Create(typeof(IService14), typeof(Service14)),
                Tuple.Create(typeof(IService15), typeof(Service15)),
                Tuple.Create(typeof(IService16), typeof(Service16)),
                Tuple.Create(typeof(IService17), typeof(Service17)),
                Tuple.Create(typeof(IService18), typeof(Service18)),
                Tuple.Create(typeof(IService19), typeof(Service19)),
                Tuple.Create(typeof(IService20), typeof(Service20)),
                Tuple.Create(typeof(IService21), typeof(Service21)),
                Tuple.Create(typeof(IService22), typeof(Service22)),
                Tuple.Create(typeof(IService23), typeof(Service23)),
                Tuple.Create(typeof(IService24), typeof(Service24)),
                Tuple.Create(typeof(IService25), typeof(Service25)),
                Tuple.Create(typeof(IService26), typeof(Service26)),
                Tuple.Create(typeof(IService27), typeof(Service27)),
                Tuple.Create(typeof(IService28), typeof(Service28)),
                Tuple.Create(typeof(IService29), typeof(Service29)),
                Tuple.Create(typeof(IService30), typeof(Service30)),
                Tuple.Create(typeof(IService31), typeof(Service31)),
                Tuple.Create(typeof(IService32), typeof(Service32)),
                Tuple.Create(typeof(IService33), typeof(Service33)),
                Tuple.Create(typeof(IService34), typeof(Service34)),
                Tuple.Create(typeof(IService35), typeof(Service35)),
                Tuple.Create(typeof(IService36), typeof(Service36)),
                Tuple.Create(typeof(IService37), typeof(Service37)),
                Tuple.Create(typeof(IService38), typeof(Service38)),
                Tuple.Create(typeof(IService39), typeof(Service39)),
                Tuple.Create(typeof(IService40), typeof(Service40)),
                Tuple.Create(typeof(IService41), typeof(Service41)),
                Tuple.Create(typeof(IService42), typeof(Service42)),
                Tuple.Create(typeof(IService43), typeof(Service43)),
                Tuple.Create(typeof(IService44), typeof(Service44)),
                Tuple.Create(typeof(IService45), typeof(Service45)),
                Tuple.Create(typeof(IService46), typeof(Service46)),
                Tuple.Create(typeof(IService47), typeof(Service47)),
                Tuple.Create(typeof(IService48), typeof(Service48)),
                Tuple.Create(typeof(IService49), typeof(Service49)),
                Tuple.Create(typeof(IService50), typeof(Service50)),
                Tuple.Create(typeof(IService51), typeof(Service51)),
                Tuple.Create(typeof(IService52), typeof(Service52)),
                Tuple.Create(typeof(IService53), typeof(Service53)),
                Tuple.Create(typeof(IService54), typeof(Service54)),
                Tuple.Create(typeof(IService55), typeof(Service55)),
                Tuple.Create(typeof(IService56), typeof(Service56)),
                Tuple.Create(typeof(IService57), typeof(Service57)),
                Tuple.Create(typeof(IService58), typeof(Service58)),
                Tuple.Create(typeof(IService59), typeof(Service59)),
                Tuple.Create(typeof(IService60), typeof(Service60)),
                Tuple.Create(typeof(IService61), typeof(Service61)),
                Tuple.Create(typeof(IService62), typeof(Service62)),
                Tuple.Create(typeof(IService63), typeof(Service63)),
                Tuple.Create(typeof(IService64), typeof(Service64)),
                Tuple.Create(typeof(IService65), typeof(Service65)),
                Tuple.Create(typeof(IService66), typeof(Service66)),
                Tuple.Create(typeof(IService67), typeof(Service67)),
                Tuple.Create(typeof(IService68), typeof(Service68)),
                Tuple.Create(typeof(IService69), typeof(Service69)),
                Tuple.Create(typeof(IService70), typeof(Service70)),
                Tuple.Create(typeof(IService71), typeof(Service71)),
                Tuple.Create(typeof(IService72), typeof(Service72)),
                Tuple.Create(typeof(IService73), typeof(Service73)),
                Tuple.Create(typeof(IService74), typeof(Service74)),
                Tuple.Create(typeof(IService75), typeof(Service75)),
                Tuple.Create(typeof(IService76), typeof(Service76)),
                Tuple.Create(typeof(IService77), typeof(Service77)),
                Tuple.Create(typeof(IService78), typeof(Service78)),
                Tuple.Create(typeof(IService79), typeof(Service79)),
                Tuple.Create(typeof(IService80), typeof(Service80)),
                Tuple.Create(typeof(IService81), typeof(Service81)),
                Tuple.Create(typeof(IService82), typeof(Service82)),
                Tuple.Create(typeof(IService83), typeof(Service83)),
                Tuple.Create(typeof(IService84), typeof(Service84)),
                Tuple.Create(typeof(IService85), typeof(Service85)),
                Tuple.Create(typeof(IService86), typeof(Service86)),
                Tuple.Create(typeof(IService87), typeof(Service87)),
                Tuple.Create(typeof(IService88), typeof(Service88)),
                Tuple.Create(typeof(IService89), typeof(Service89)),
                Tuple.Create(typeof(IService90), typeof(Service90)),
                Tuple.Create(typeof(IService91), typeof(Service91)),
                Tuple.Create(typeof(IService92), typeof(Service92)),
                Tuple.Create(typeof(IService93), typeof(Service93)),
                Tuple.Create(typeof(IService94), typeof(Service94)),
                Tuple.Create(typeof(IService95), typeof(Service95)),
                Tuple.Create(typeof(IService96), typeof(Service96)),
                Tuple.Create(typeof(IService97), typeof(Service97)),
                Tuple.Create(typeof(IService98), typeof(Service98)),
                Tuple.Create(typeof(IService99), typeof(Service99)),
                Tuple.Create(typeof(IService100), typeof(Service100)),
                Tuple.Create(typeof(IService101), typeof(Service101)),
                Tuple.Create(typeof(IService102), typeof(Service102)),
                Tuple.Create(typeof(IService103), typeof(Service103)),
                Tuple.Create(typeof(IService104), typeof(Service104)),
                Tuple.Create(typeof(IService105), typeof(Service105)),
                Tuple.Create(typeof(IService106), typeof(Service106)),
                Tuple.Create(typeof(IService107), typeof(Service107)),
                Tuple.Create(typeof(IService108), typeof(Service108)),
                Tuple.Create(typeof(IService109), typeof(Service109)),
                Tuple.Create(typeof(IService110), typeof(Service110)),
                Tuple.Create(typeof(IService111), typeof(Service111)),
                Tuple.Create(typeof(IService112), typeof(Service112)),
                Tuple.Create(typeof(IService113), typeof(Service113)),
                Tuple.Create(typeof(IService114), typeof(Service114)),
                Tuple.Create(typeof(IService115), typeof(Service115)),
                Tuple.Create(typeof(IService116), typeof(Service116)),
                Tuple.Create(typeof(IService117), typeof(Service117)),
                Tuple.Create(typeof(IService118), typeof(Service118)),
                Tuple.Create(typeof(IService119), typeof(Service119)),
                Tuple.Create(typeof(IService120), typeof(Service120)),
                Tuple.Create(typeof(IService121), typeof(Service121)),
                Tuple.Create(typeof(IService122), typeof(Service122)),
                Tuple.Create(typeof(IService123), typeof(Service123)),
                Tuple.Create(typeof(IService124), typeof(Service124)),
                Tuple.Create(typeof(IService125), typeof(Service125)),
                Tuple.Create(typeof(IService126), typeof(Service126)),
                Tuple.Create(typeof(IService127), typeof(Service127)),
                Tuple.Create(typeof(IService128), typeof(Service128)),
                Tuple.Create(typeof(IService129), typeof(Service129)),
                Tuple.Create(typeof(IService130), typeof(Service130)),
                Tuple.Create(typeof(IService131), typeof(Service131)),
                Tuple.Create(typeof(IService132), typeof(Service132)),
                Tuple.Create(typeof(IService133), typeof(Service133)),
                Tuple.Create(typeof(IService134), typeof(Service134)),
                Tuple.Create(typeof(IService135), typeof(Service135)),
                Tuple.Create(typeof(IService136), typeof(Service136)),
                Tuple.Create(typeof(IService137), typeof(Service137)),
                Tuple.Create(typeof(IService138), typeof(Service138)),
                Tuple.Create(typeof(IService139), typeof(Service139)),
                Tuple.Create(typeof(IService140), typeof(Service140)),
                Tuple.Create(typeof(IService141), typeof(Service141)),
                Tuple.Create(typeof(IService142), typeof(Service142)),
                Tuple.Create(typeof(IService143), typeof(Service143)),
                Tuple.Create(typeof(IService144), typeof(Service144)),
                Tuple.Create(typeof(IService145), typeof(Service145)),
                Tuple.Create(typeof(IService146), typeof(Service146)),
                Tuple.Create(typeof(IService147), typeof(Service147)),
                Tuple.Create(typeof(IService148), typeof(Service148)),
                Tuple.Create(typeof(IService149), typeof(Service149)),
                Tuple.Create(typeof(IService150), typeof(Service150)),
                Tuple.Create(typeof(IService151), typeof(Service151)),
                Tuple.Create(typeof(IService152), typeof(Service152)),
                Tuple.Create(typeof(IService153), typeof(Service153)),
                Tuple.Create(typeof(IService154), typeof(Service154)),
                Tuple.Create(typeof(IService155), typeof(Service155)),
                Tuple.Create(typeof(IService156), typeof(Service156)),
                Tuple.Create(typeof(IService157), typeof(Service157)),
                Tuple.Create(typeof(IService158), typeof(Service158)),
                Tuple.Create(typeof(IService159), typeof(Service159)),
                Tuple.Create(typeof(IService160), typeof(Service160)),
                Tuple.Create(typeof(IService161), typeof(Service161)),
                Tuple.Create(typeof(IService162), typeof(Service162)),
                Tuple.Create(typeof(IService163), typeof(Service163)),
                Tuple.Create(typeof(IService164), typeof(Service164)),
                Tuple.Create(typeof(IService165), typeof(Service165)),
                Tuple.Create(typeof(IService166), typeof(Service166)),
                Tuple.Create(typeof(IService167), typeof(Service167)),
                Tuple.Create(typeof(IService168), typeof(Service168)),
                Tuple.Create(typeof(IService169), typeof(Service169)),
                Tuple.Create(typeof(IService170), typeof(Service170)),
                Tuple.Create(typeof(IService171), typeof(Service171)),
                Tuple.Create(typeof(IService172), typeof(Service172)),
                Tuple.Create(typeof(IService173), typeof(Service173)),
                Tuple.Create(typeof(IService174), typeof(Service174)),
                Tuple.Create(typeof(IService175), typeof(Service175)),
                Tuple.Create(typeof(IService176), typeof(Service176)),
                Tuple.Create(typeof(IService177), typeof(Service177)),
                Tuple.Create(typeof(IService178), typeof(Service178)),
                Tuple.Create(typeof(IService179), typeof(Service179)),
                Tuple.Create(typeof(IService180), typeof(Service180)),
                Tuple.Create(typeof(IService181), typeof(Service181)),
                Tuple.Create(typeof(IService182), typeof(Service182)),
                Tuple.Create(typeof(IService183), typeof(Service183)),
                Tuple.Create(typeof(IService184), typeof(Service184)),
                Tuple.Create(typeof(IService185), typeof(Service185)),
                Tuple.Create(typeof(IService186), typeof(Service186)),
                Tuple.Create(typeof(IService187), typeof(Service187)),
                Tuple.Create(typeof(IService188), typeof(Service188)),
                Tuple.Create(typeof(IService189), typeof(Service189)),
                Tuple.Create(typeof(IService190), typeof(Service190)),
                Tuple.Create(typeof(IService191), typeof(Service191)),
                Tuple.Create(typeof(IService192), typeof(Service192)),
                Tuple.Create(typeof(IService193), typeof(Service193)),
                Tuple.Create(typeof(IService194), typeof(Service194)),
                Tuple.Create(typeof(IService195), typeof(Service195)),
                Tuple.Create(typeof(IService196), typeof(Service196)),
                Tuple.Create(typeof(IService197), typeof(Service197)),
                Tuple.Create(typeof(IService198), typeof(Service198)),
                Tuple.Create(typeof(IService199), typeof(Service199)),
                Tuple.Create(typeof(IService200), typeof(Service200)),
                Tuple.Create(typeof(IService201), typeof(Service201)),
                Tuple.Create(typeof(IService202), typeof(Service202)),
                Tuple.Create(typeof(IService203), typeof(Service203)),
                Tuple.Create(typeof(IService204), typeof(Service204)),
                Tuple.Create(typeof(IService205), typeof(Service205)),
                Tuple.Create(typeof(IService206), typeof(Service206)),
                Tuple.Create(typeof(IService207), typeof(Service207)),
                Tuple.Create(typeof(IService208), typeof(Service208)),
                Tuple.Create(typeof(IService209), typeof(Service209)),
                Tuple.Create(typeof(IService210), typeof(Service210)),
                Tuple.Create(typeof(IService211), typeof(Service211)),
                Tuple.Create(typeof(IService212), typeof(Service212)),
                Tuple.Create(typeof(IService213), typeof(Service213)),
                Tuple.Create(typeof(IService214), typeof(Service214)),
                Tuple.Create(typeof(IService215), typeof(Service215)),
                Tuple.Create(typeof(IService216), typeof(Service216)),
                Tuple.Create(typeof(IService217), typeof(Service217)),
                Tuple.Create(typeof(IService218), typeof(Service218)),
                Tuple.Create(typeof(IService219), typeof(Service219)),
                Tuple.Create(typeof(IService220), typeof(Service220)),
                Tuple.Create(typeof(IService221), typeof(Service221)),
                Tuple.Create(typeof(IService222), typeof(Service222)),
                Tuple.Create(typeof(IService223), typeof(Service223)),
                Tuple.Create(typeof(IService224), typeof(Service224)),
                Tuple.Create(typeof(IService225), typeof(Service225)),
                Tuple.Create(typeof(IService226), typeof(Service226)),
                Tuple.Create(typeof(IService227), typeof(Service227)),
                Tuple.Create(typeof(IService228), typeof(Service228)),
                Tuple.Create(typeof(IService229), typeof(Service229)),
                Tuple.Create(typeof(IService230), typeof(Service230)),
                Tuple.Create(typeof(IService231), typeof(Service231)),
                Tuple.Create(typeof(IService232), typeof(Service232)),
                Tuple.Create(typeof(IService233), typeof(Service233)),
                Tuple.Create(typeof(IService234), typeof(Service234)),
                Tuple.Create(typeof(IService235), typeof(Service235)),
                Tuple.Create(typeof(IService236), typeof(Service236)),
                Tuple.Create(typeof(IService237), typeof(Service237)),
                Tuple.Create(typeof(IService238), typeof(Service238)),
                Tuple.Create(typeof(IService239), typeof(Service239)),
                Tuple.Create(typeof(IService240), typeof(Service240)),
                Tuple.Create(typeof(IService241), typeof(Service241)),
                Tuple.Create(typeof(IService242), typeof(Service242)),
                Tuple.Create(typeof(IService243), typeof(Service243)),
                Tuple.Create(typeof(IService244), typeof(Service244)),
                Tuple.Create(typeof(IService245), typeof(Service245)),
                Tuple.Create(typeof(IService246), typeof(Service246)),
                Tuple.Create(typeof(IService247), typeof(Service247)),
                Tuple.Create(typeof(IService248), typeof(Service248)),
                Tuple.Create(typeof(IService249), typeof(Service249)),
                Tuple.Create(typeof(IService250), typeof(Service250)),
                Tuple.Create(typeof(IService251), typeof(Service251)),
                Tuple.Create(typeof(IService252), typeof(Service252)),
                Tuple.Create(typeof(IService253), typeof(Service253)),
                Tuple.Create(typeof(IService254), typeof(Service254)),
                Tuple.Create(typeof(IService255), typeof(Service255)),
                Tuple.Create(typeof(IService256), typeof(Service256)),
                Tuple.Create(typeof(IService257), typeof(Service257)),
                Tuple.Create(typeof(IService258), typeof(Service258)),
                Tuple.Create(typeof(IService259), typeof(Service259)),
                Tuple.Create(typeof(IService260), typeof(Service260)),
                Tuple.Create(typeof(IService261), typeof(Service261)),
                Tuple.Create(typeof(IService262), typeof(Service262)),
                Tuple.Create(typeof(IService263), typeof(Service263)),
                Tuple.Create(typeof(IService264), typeof(Service264)),
                Tuple.Create(typeof(IService265), typeof(Service265)),
                Tuple.Create(typeof(IService266), typeof(Service266)),
                Tuple.Create(typeof(IService267), typeof(Service267)),
                Tuple.Create(typeof(IService268), typeof(Service268)),
                Tuple.Create(typeof(IService269), typeof(Service269)),
                Tuple.Create(typeof(IService270), typeof(Service270)),
                Tuple.Create(typeof(IService271), typeof(Service271)),
                Tuple.Create(typeof(IService272), typeof(Service272)),
                Tuple.Create(typeof(IService273), typeof(Service273)),
                Tuple.Create(typeof(IService274), typeof(Service274)),
                Tuple.Create(typeof(IService275), typeof(Service275)),
                Tuple.Create(typeof(IService276), typeof(Service276)),
                Tuple.Create(typeof(IService277), typeof(Service277)),
                Tuple.Create(typeof(IService278), typeof(Service278)),
                Tuple.Create(typeof(IService279), typeof(Service279)),
                Tuple.Create(typeof(IService280), typeof(Service280)),
                Tuple.Create(typeof(IService281), typeof(Service281)),
                Tuple.Create(typeof(IService282), typeof(Service282)),
                Tuple.Create(typeof(IService283), typeof(Service283)),
                Tuple.Create(typeof(IService284), typeof(Service284)),
                Tuple.Create(typeof(IService285), typeof(Service285)),
                Tuple.Create(typeof(IService286), typeof(Service286)),
                Tuple.Create(typeof(IService287), typeof(Service287)),
                Tuple.Create(typeof(IService288), typeof(Service288)),
                Tuple.Create(typeof(IService289), typeof(Service289)),
                Tuple.Create(typeof(IService290), typeof(Service290)),
                Tuple.Create(typeof(IService291), typeof(Service291)),
                Tuple.Create(typeof(IService292), typeof(Service292)),
                Tuple.Create(typeof(IService293), typeof(Service293)),
                Tuple.Create(typeof(IService294), typeof(Service294)),
                Tuple.Create(typeof(IService295), typeof(Service295)),
                Tuple.Create(typeof(IService296), typeof(Service296)),
                Tuple.Create(typeof(IService297), typeof(Service297)),
                Tuple.Create(typeof(IService298), typeof(Service298)),
                Tuple.Create(typeof(IService299), typeof(Service299)),
                Tuple.Create(typeof(IService300), typeof(Service300)),
                Tuple.Create(typeof(IService301), typeof(Service301)),
                Tuple.Create(typeof(IService302), typeof(Service302)),
                Tuple.Create(typeof(IService303), typeof(Service303)),
                Tuple.Create(typeof(IService304), typeof(Service304)),
                Tuple.Create(typeof(IService305), typeof(Service305)),
                Tuple.Create(typeof(IService306), typeof(Service306)),
                Tuple.Create(typeof(IService307), typeof(Service307)),
                Tuple.Create(typeof(IService308), typeof(Service308)),
                Tuple.Create(typeof(IService309), typeof(Service309)),
                Tuple.Create(typeof(IService310), typeof(Service310)),
                Tuple.Create(typeof(IService311), typeof(Service311)),
                Tuple.Create(typeof(IService312), typeof(Service312)),
                Tuple.Create(typeof(IService313), typeof(Service313)),
                Tuple.Create(typeof(IService314), typeof(Service314)),
                Tuple.Create(typeof(IService315), typeof(Service315)),
                Tuple.Create(typeof(IService316), typeof(Service316)),
                Tuple.Create(typeof(IService317), typeof(Service317)),
                Tuple.Create(typeof(IService318), typeof(Service318)),
                Tuple.Create(typeof(IService319), typeof(Service319)),
                Tuple.Create(typeof(IService320), typeof(Service320)),
                Tuple.Create(typeof(IService321), typeof(Service321)),
                Tuple.Create(typeof(IService322), typeof(Service322)),
                Tuple.Create(typeof(IService323), typeof(Service323)),
                Tuple.Create(typeof(IService324), typeof(Service324)),
                Tuple.Create(typeof(IService325), typeof(Service325)),
                Tuple.Create(typeof(IService326), typeof(Service326)),
                Tuple.Create(typeof(IService327), typeof(Service327)),
                Tuple.Create(typeof(IService328), typeof(Service328)),
                Tuple.Create(typeof(IService329), typeof(Service329)),
                Tuple.Create(typeof(IService330), typeof(Service330)),
                Tuple.Create(typeof(IService331), typeof(Service331)),
                Tuple.Create(typeof(IService332), typeof(Service332)),
                Tuple.Create(typeof(IService333), typeof(Service333)),
                Tuple.Create(typeof(IService334), typeof(Service334)),
                Tuple.Create(typeof(IService335), typeof(Service335)),
                Tuple.Create(typeof(IService336), typeof(Service336)),
                Tuple.Create(typeof(IService337), typeof(Service337)),
                Tuple.Create(typeof(IService338), typeof(Service338)),
                Tuple.Create(typeof(IService339), typeof(Service339)),
                Tuple.Create(typeof(IService340), typeof(Service340)),
                Tuple.Create(typeof(IService341), typeof(Service341)),
                Tuple.Create(typeof(IService342), typeof(Service342)),
                Tuple.Create(typeof(IService343), typeof(Service343)),
                Tuple.Create(typeof(IService344), typeof(Service344)),
                Tuple.Create(typeof(IService345), typeof(Service345)),
                Tuple.Create(typeof(IService346), typeof(Service346)),
                Tuple.Create(typeof(IService347), typeof(Service347)),
                Tuple.Create(typeof(IService348), typeof(Service348)),
                Tuple.Create(typeof(IService349), typeof(Service349)),
                Tuple.Create(typeof(IService350), typeof(Service350)),
                Tuple.Create(typeof(IService351), typeof(Service351)),
                Tuple.Create(typeof(IService352), typeof(Service352)),
                Tuple.Create(typeof(IService353), typeof(Service353)),
                Tuple.Create(typeof(IService354), typeof(Service354)),
                Tuple.Create(typeof(IService355), typeof(Service355)),
                Tuple.Create(typeof(IService356), typeof(Service356)),
                Tuple.Create(typeof(IService357), typeof(Service357)),
                Tuple.Create(typeof(IService358), typeof(Service358)),
                Tuple.Create(typeof(IService359), typeof(Service359)),
                Tuple.Create(typeof(IService360), typeof(Service360)),
                Tuple.Create(typeof(IService361), typeof(Service361)),
                Tuple.Create(typeof(IService362), typeof(Service362)),
                Tuple.Create(typeof(IService363), typeof(Service363)),
                Tuple.Create(typeof(IService364), typeof(Service364)),
                Tuple.Create(typeof(IService365), typeof(Service365)),
                Tuple.Create(typeof(IService366), typeof(Service366)),
                Tuple.Create(typeof(IService367), typeof(Service367)),
                Tuple.Create(typeof(IService368), typeof(Service368)),
                Tuple.Create(typeof(IService369), typeof(Service369)),
                Tuple.Create(typeof(IService370), typeof(Service370)),
                Tuple.Create(typeof(IService371), typeof(Service371)),
                Tuple.Create(typeof(IService372), typeof(Service372)),
                Tuple.Create(typeof(IService373), typeof(Service373)),
                Tuple.Create(typeof(IService374), typeof(Service374)),
                Tuple.Create(typeof(IService375), typeof(Service375)),
                Tuple.Create(typeof(IService376), typeof(Service376)),
                Tuple.Create(typeof(IService377), typeof(Service377)),
                Tuple.Create(typeof(IService378), typeof(Service378)),
                Tuple.Create(typeof(IService379), typeof(Service379)),
                Tuple.Create(typeof(IService380), typeof(Service380)),
                Tuple.Create(typeof(IService381), typeof(Service381)),
                Tuple.Create(typeof(IService382), typeof(Service382)),
                Tuple.Create(typeof(IService383), typeof(Service383)),
                Tuple.Create(typeof(IService384), typeof(Service384)),
                Tuple.Create(typeof(IService385), typeof(Service385)),
                Tuple.Create(typeof(IService386), typeof(Service386)),
                Tuple.Create(typeof(IService387), typeof(Service387)),
                Tuple.Create(typeof(IService388), typeof(Service388)),
                Tuple.Create(typeof(IService389), typeof(Service389)),
                Tuple.Create(typeof(IService390), typeof(Service390)),
                Tuple.Create(typeof(IService391), typeof(Service391)),
                Tuple.Create(typeof(IService392), typeof(Service392)),
                Tuple.Create(typeof(IService393), typeof(Service393)),
                Tuple.Create(typeof(IService394), typeof(Service394)),
                Tuple.Create(typeof(IService395), typeof(Service395)),
                Tuple.Create(typeof(IService396), typeof(Service396)),
                Tuple.Create(typeof(IService397), typeof(Service397)),
                Tuple.Create(typeof(IService398), typeof(Service398)),
                Tuple.Create(typeof(IService399), typeof(Service399)),
                Tuple.Create(typeof(IService400), typeof(Service400)),
                Tuple.Create(typeof(IService401), typeof(Service401)),
                Tuple.Create(typeof(IService402), typeof(Service402)),
                Tuple.Create(typeof(IService403), typeof(Service403)),
                Tuple.Create(typeof(IService404), typeof(Service404)),
                Tuple.Create(typeof(IService405), typeof(Service405)),
                Tuple.Create(typeof(IService406), typeof(Service406)),
                Tuple.Create(typeof(IService407), typeof(Service407)),
                Tuple.Create(typeof(IService408), typeof(Service408)),
                Tuple.Create(typeof(IService409), typeof(Service409)),
                Tuple.Create(typeof(IService410), typeof(Service410)),
                Tuple.Create(typeof(IService411), typeof(Service411)),
                Tuple.Create(typeof(IService412), typeof(Service412)),
                Tuple.Create(typeof(IService413), typeof(Service413)),
                Tuple.Create(typeof(IService414), typeof(Service414)),
                Tuple.Create(typeof(IService415), typeof(Service415)),
                Tuple.Create(typeof(IService416), typeof(Service416)),
                Tuple.Create(typeof(IService417), typeof(Service417)),
                Tuple.Create(typeof(IService418), typeof(Service418)),
                Tuple.Create(typeof(IService419), typeof(Service419)),
                Tuple.Create(typeof(IService420), typeof(Service420)),
                Tuple.Create(typeof(IService421), typeof(Service421)),
                Tuple.Create(typeof(IService422), typeof(Service422)),
                Tuple.Create(typeof(IService423), typeof(Service423)),
                Tuple.Create(typeof(IService424), typeof(Service424)),
                Tuple.Create(typeof(IService425), typeof(Service425)),
                Tuple.Create(typeof(IService426), typeof(Service426)),
                Tuple.Create(typeof(IService427), typeof(Service427)),
                Tuple.Create(typeof(IService428), typeof(Service428)),
                Tuple.Create(typeof(IService429), typeof(Service429)),
                Tuple.Create(typeof(IService430), typeof(Service430)),
                Tuple.Create(typeof(IService431), typeof(Service431)),
                Tuple.Create(typeof(IService432), typeof(Service432)),
                Tuple.Create(typeof(IService433), typeof(Service433)),
                Tuple.Create(typeof(IService434), typeof(Service434)),
                Tuple.Create(typeof(IService435), typeof(Service435)),
                Tuple.Create(typeof(IService436), typeof(Service436)),
                Tuple.Create(typeof(IService437), typeof(Service437)),
                Tuple.Create(typeof(IService438), typeof(Service438)),
                Tuple.Create(typeof(IService439), typeof(Service439)),
                Tuple.Create(typeof(IService440), typeof(Service440)),
                Tuple.Create(typeof(IService441), typeof(Service441)),
                Tuple.Create(typeof(IService442), typeof(Service442)),
                Tuple.Create(typeof(IService443), typeof(Service443)),
                Tuple.Create(typeof(IService444), typeof(Service444)),
                Tuple.Create(typeof(IService445), typeof(Service445)),
                Tuple.Create(typeof(IService446), typeof(Service446)),
                Tuple.Create(typeof(IService447), typeof(Service447)),
                Tuple.Create(typeof(IService448), typeof(Service448)),
                Tuple.Create(typeof(IService449), typeof(Service449)),
                Tuple.Create(typeof(IService450), typeof(Service450)),
                Tuple.Create(typeof(IService451), typeof(Service451)),
                Tuple.Create(typeof(IService452), typeof(Service452)),
                Tuple.Create(typeof(IService453), typeof(Service453)),
                Tuple.Create(typeof(IService454), typeof(Service454)),
                Tuple.Create(typeof(IService455), typeof(Service455)),
                Tuple.Create(typeof(IService456), typeof(Service456)),
                Tuple.Create(typeof(IService457), typeof(Service457)),
                Tuple.Create(typeof(IService458), typeof(Service458)),
                Tuple.Create(typeof(IService459), typeof(Service459)),
                Tuple.Create(typeof(IService460), typeof(Service460)),
                Tuple.Create(typeof(IService461), typeof(Service461)),
                Tuple.Create(typeof(IService462), typeof(Service462)),
                Tuple.Create(typeof(IService463), typeof(Service463)),
                Tuple.Create(typeof(IService464), typeof(Service464)),
                Tuple.Create(typeof(IService465), typeof(Service465)),
                Tuple.Create(typeof(IService466), typeof(Service466)),
                Tuple.Create(typeof(IService467), typeof(Service467)),
                Tuple.Create(typeof(IService468), typeof(Service468)),
                Tuple.Create(typeof(IService469), typeof(Service469)),
                Tuple.Create(typeof(IService470), typeof(Service470)),
                Tuple.Create(typeof(IService471), typeof(Service471)),
                Tuple.Create(typeof(IService472), typeof(Service472)),
                Tuple.Create(typeof(IService473), typeof(Service473)),
                Tuple.Create(typeof(IService474), typeof(Service474)),
                Tuple.Create(typeof(IService475), typeof(Service475)),
                Tuple.Create(typeof(IService476), typeof(Service476)),
                Tuple.Create(typeof(IService477), typeof(Service477)),
                Tuple.Create(typeof(IService478), typeof(Service478)),
                Tuple.Create(typeof(IService479), typeof(Service479)),
                Tuple.Create(typeof(IService480), typeof(Service480)),
                Tuple.Create(typeof(IService481), typeof(Service481)),
                Tuple.Create(typeof(IService482), typeof(Service482)),
                Tuple.Create(typeof(IService483), typeof(Service483)),
                Tuple.Create(typeof(IService484), typeof(Service484)),
                Tuple.Create(typeof(IService485), typeof(Service485)),
                Tuple.Create(typeof(IService486), typeof(Service486)),
                Tuple.Create(typeof(IService487), typeof(Service487)),
                Tuple.Create(typeof(IService488), typeof(Service488)),
                Tuple.Create(typeof(IService489), typeof(Service489)),
                Tuple.Create(typeof(IService490), typeof(Service490)),
                Tuple.Create(typeof(IService491), typeof(Service491)),
                Tuple.Create(typeof(IService492), typeof(Service492)),
                Tuple.Create(typeof(IService493), typeof(Service493)),
                Tuple.Create(typeof(IService494), typeof(Service494)),
                Tuple.Create(typeof(IService495), typeof(Service495)),
                Tuple.Create(typeof(IService496), typeof(Service496)),
                Tuple.Create(typeof(IService497), typeof(Service497)),
                Tuple.Create(typeof(IService498), typeof(Service498)),
                Tuple.Create(typeof(IService499), typeof(Service499)),
                Tuple.Create(typeof(IService500), typeof(Service500)),
                Tuple.Create(typeof(IService501), typeof(Service501)),
                Tuple.Create(typeof(IService502), typeof(Service502)),
                Tuple.Create(typeof(IService503), typeof(Service503)),
                Tuple.Create(typeof(IService504), typeof(Service504)),
                Tuple.Create(typeof(IService505), typeof(Service505)),
                Tuple.Create(typeof(IService506), typeof(Service506)),
                Tuple.Create(typeof(IService507), typeof(Service507)),
                Tuple.Create(typeof(IService508), typeof(Service508)),
                Tuple.Create(typeof(IService509), typeof(Service509)),
                Tuple.Create(typeof(IService510), typeof(Service510)),
                Tuple.Create(typeof(IService511), typeof(Service511)),
                Tuple.Create(typeof(IService512), typeof(Service512)),
                Tuple.Create(typeof(IService513), typeof(Service513)),
                Tuple.Create(typeof(IService514), typeof(Service514)),
                Tuple.Create(typeof(IService515), typeof(Service515)),
                Tuple.Create(typeof(IService516), typeof(Service516)),
                Tuple.Create(typeof(IService517), typeof(Service517)),
                Tuple.Create(typeof(IService518), typeof(Service518)),
                Tuple.Create(typeof(IService519), typeof(Service519)),
                Tuple.Create(typeof(IService520), typeof(Service520)),
                Tuple.Create(typeof(IService521), typeof(Service521)),
                Tuple.Create(typeof(IService522), typeof(Service522)),
                Tuple.Create(typeof(IService523), typeof(Service523)),
                Tuple.Create(typeof(IService524), typeof(Service524)),
                Tuple.Create(typeof(IService525), typeof(Service525)),
                Tuple.Create(typeof(IService526), typeof(Service526)),
                Tuple.Create(typeof(IService527), typeof(Service527)),
                Tuple.Create(typeof(IService528), typeof(Service528)),
                Tuple.Create(typeof(IService529), typeof(Service529)),
                Tuple.Create(typeof(IService530), typeof(Service530)),
                Tuple.Create(typeof(IService531), typeof(Service531)),
                Tuple.Create(typeof(IService532), typeof(Service532)),
                Tuple.Create(typeof(IService533), typeof(Service533)),
                Tuple.Create(typeof(IService534), typeof(Service534)),
                Tuple.Create(typeof(IService535), typeof(Service535)),
                Tuple.Create(typeof(IService536), typeof(Service536)),
                Tuple.Create(typeof(IService537), typeof(Service537)),
                Tuple.Create(typeof(IService538), typeof(Service538)),
                Tuple.Create(typeof(IService539), typeof(Service539)),
                Tuple.Create(typeof(IService540), typeof(Service540)),
                Tuple.Create(typeof(IService541), typeof(Service541)),
                Tuple.Create(typeof(IService542), typeof(Service542)),
                Tuple.Create(typeof(IService543), typeof(Service543)),
                Tuple.Create(typeof(IService544), typeof(Service544)),
                Tuple.Create(typeof(IService545), typeof(Service545)),
                Tuple.Create(typeof(IService546), typeof(Service546)),
                Tuple.Create(typeof(IService547), typeof(Service547)),
                Tuple.Create(typeof(IService548), typeof(Service548)),
                Tuple.Create(typeof(IService549), typeof(Service549)),
                Tuple.Create(typeof(IService550), typeof(Service550)),
                Tuple.Create(typeof(IService551), typeof(Service551)),
                Tuple.Create(typeof(IService552), typeof(Service552)),
                Tuple.Create(typeof(IService553), typeof(Service553)),
                Tuple.Create(typeof(IService554), typeof(Service554)),
                Tuple.Create(typeof(IService555), typeof(Service555)),
                Tuple.Create(typeof(IService556), typeof(Service556)),
                Tuple.Create(typeof(IService557), typeof(Service557)),
                Tuple.Create(typeof(IService558), typeof(Service558)),
                Tuple.Create(typeof(IService559), typeof(Service559)),
                Tuple.Create(typeof(IService560), typeof(Service560)),
                Tuple.Create(typeof(IService561), typeof(Service561)),
                Tuple.Create(typeof(IService562), typeof(Service562)),
                Tuple.Create(typeof(IService563), typeof(Service563)),
                Tuple.Create(typeof(IService564), typeof(Service564)),
                Tuple.Create(typeof(IService565), typeof(Service565)),
                Tuple.Create(typeof(IService566), typeof(Service566)),
                Tuple.Create(typeof(IService567), typeof(Service567)),
                Tuple.Create(typeof(IService568), typeof(Service568)),
                Tuple.Create(typeof(IService569), typeof(Service569)),
                Tuple.Create(typeof(IService570), typeof(Service570)),
                Tuple.Create(typeof(IService571), typeof(Service571)),
                Tuple.Create(typeof(IService572), typeof(Service572)),
                Tuple.Create(typeof(IService573), typeof(Service573)),
                Tuple.Create(typeof(IService574), typeof(Service574)),
                Tuple.Create(typeof(IService575), typeof(Service575)),
                Tuple.Create(typeof(IService576), typeof(Service576)),
                Tuple.Create(typeof(IService577), typeof(Service577)),
                Tuple.Create(typeof(IService578), typeof(Service578)),
                Tuple.Create(typeof(IService579), typeof(Service579)),
                Tuple.Create(typeof(IService580), typeof(Service580)),
                Tuple.Create(typeof(IService581), typeof(Service581)),
                Tuple.Create(typeof(IService582), typeof(Service582)),
                Tuple.Create(typeof(IService583), typeof(Service583)),
                Tuple.Create(typeof(IService584), typeof(Service584)),
                Tuple.Create(typeof(IService585), typeof(Service585)),
                Tuple.Create(typeof(IService586), typeof(Service586)),
                Tuple.Create(typeof(IService587), typeof(Service587)),
                Tuple.Create(typeof(IService588), typeof(Service588)),
                Tuple.Create(typeof(IService589), typeof(Service589)),
                Tuple.Create(typeof(IService590), typeof(Service590)),
                Tuple.Create(typeof(IService591), typeof(Service591)),
                Tuple.Create(typeof(IService592), typeof(Service592)),
                Tuple.Create(typeof(IService593), typeof(Service593)),
                Tuple.Create(typeof(IService594), typeof(Service594)),
                Tuple.Create(typeof(IService595), typeof(Service595)),
                Tuple.Create(typeof(IService596), typeof(Service596)),
                Tuple.Create(typeof(IService597), typeof(Service597)),
                Tuple.Create(typeof(IService598), typeof(Service598)),
                Tuple.Create(typeof(IService599), typeof(Service599)),
                Tuple.Create(typeof(IService600), typeof(Service600)),
                Tuple.Create(typeof(IService601), typeof(Service601)),
                Tuple.Create(typeof(IService602), typeof(Service602)),
                Tuple.Create(typeof(IService603), typeof(Service603)),
                Tuple.Create(typeof(IService604), typeof(Service604)),
                Tuple.Create(typeof(IService605), typeof(Service605)),
                Tuple.Create(typeof(IService606), typeof(Service606)),
                Tuple.Create(typeof(IService607), typeof(Service607)),
                Tuple.Create(typeof(IService608), typeof(Service608)),
                Tuple.Create(typeof(IService609), typeof(Service609)),
                Tuple.Create(typeof(IService610), typeof(Service610)),
                Tuple.Create(typeof(IService611), typeof(Service611)),
                Tuple.Create(typeof(IService612), typeof(Service612)),
                Tuple.Create(typeof(IService613), typeof(Service613)),
                Tuple.Create(typeof(IService614), typeof(Service614)),
                Tuple.Create(typeof(IService615), typeof(Service615)),
                Tuple.Create(typeof(IService616), typeof(Service616)),
                Tuple.Create(typeof(IService617), typeof(Service617)),
                Tuple.Create(typeof(IService618), typeof(Service618)),
                Tuple.Create(typeof(IService619), typeof(Service619)),
                Tuple.Create(typeof(IService620), typeof(Service620)),
                Tuple.Create(typeof(IService621), typeof(Service621)),
                Tuple.Create(typeof(IService622), typeof(Service622)),
                Tuple.Create(typeof(IService623), typeof(Service623)),
                Tuple.Create(typeof(IService624), typeof(Service624)),
                Tuple.Create(typeof(IService625), typeof(Service625)),
                Tuple.Create(typeof(IService626), typeof(Service626)),
                Tuple.Create(typeof(IService627), typeof(Service627)),
                Tuple.Create(typeof(IService628), typeof(Service628)),
                Tuple.Create(typeof(IService629), typeof(Service629)),
                Tuple.Create(typeof(IService630), typeof(Service630)),
                Tuple.Create(typeof(IService631), typeof(Service631)),
                Tuple.Create(typeof(IService632), typeof(Service632)),
                Tuple.Create(typeof(IService633), typeof(Service633)),
                Tuple.Create(typeof(IService634), typeof(Service634)),
                Tuple.Create(typeof(IService635), typeof(Service635)),
                Tuple.Create(typeof(IService636), typeof(Service636)),
                Tuple.Create(typeof(IService637), typeof(Service637)),
                Tuple.Create(typeof(IService638), typeof(Service638)),
                Tuple.Create(typeof(IService639), typeof(Service639)),
                Tuple.Create(typeof(IService640), typeof(Service640)),
                Tuple.Create(typeof(IService641), typeof(Service641)),
                Tuple.Create(typeof(IService642), typeof(Service642)),
                Tuple.Create(typeof(IService643), typeof(Service643)),
                Tuple.Create(typeof(IService644), typeof(Service644)),
                Tuple.Create(typeof(IService645), typeof(Service645)),
                Tuple.Create(typeof(IService646), typeof(Service646)),
                Tuple.Create(typeof(IService647), typeof(Service647)),
                Tuple.Create(typeof(IService648), typeof(Service648)),
                Tuple.Create(typeof(IService649), typeof(Service649)),
                Tuple.Create(typeof(IService650), typeof(Service650)),
                Tuple.Create(typeof(IService651), typeof(Service651)),
                Tuple.Create(typeof(IService652), typeof(Service652)),
                Tuple.Create(typeof(IService653), typeof(Service653)),
                Tuple.Create(typeof(IService654), typeof(Service654)),
                Tuple.Create(typeof(IService655), typeof(Service655)),
                Tuple.Create(typeof(IService656), typeof(Service656)),
                Tuple.Create(typeof(IService657), typeof(Service657)),
                Tuple.Create(typeof(IService658), typeof(Service658)),
                Tuple.Create(typeof(IService659), typeof(Service659)),
                Tuple.Create(typeof(IService660), typeof(Service660)),
                Tuple.Create(typeof(IService661), typeof(Service661)),
                Tuple.Create(typeof(IService662), typeof(Service662)),
                Tuple.Create(typeof(IService663), typeof(Service663)),
                Tuple.Create(typeof(IService664), typeof(Service664)),
                Tuple.Create(typeof(IService665), typeof(Service665)),
                Tuple.Create(typeof(IService666), typeof(Service666)),
                Tuple.Create(typeof(IService667), typeof(Service667)),
                Tuple.Create(typeof(IService668), typeof(Service668)),
                Tuple.Create(typeof(IService669), typeof(Service669)),
                Tuple.Create(typeof(IService670), typeof(Service670)),
                Tuple.Create(typeof(IService671), typeof(Service671)),
                Tuple.Create(typeof(IService672), typeof(Service672)),
                Tuple.Create(typeof(IService673), typeof(Service673)),
                Tuple.Create(typeof(IService674), typeof(Service674)),
                Tuple.Create(typeof(IService675), typeof(Service675)),
                Tuple.Create(typeof(IService676), typeof(Service676)),
                Tuple.Create(typeof(IService677), typeof(Service677)),
                Tuple.Create(typeof(IService678), typeof(Service678)),
                Tuple.Create(typeof(IService679), typeof(Service679)),
                Tuple.Create(typeof(IService680), typeof(Service680)),
                Tuple.Create(typeof(IService681), typeof(Service681)),
                Tuple.Create(typeof(IService682), typeof(Service682)),
                Tuple.Create(typeof(IService683), typeof(Service683)),
                Tuple.Create(typeof(IService684), typeof(Service684)),
                Tuple.Create(typeof(IService685), typeof(Service685)),
                Tuple.Create(typeof(IService686), typeof(Service686)),
                Tuple.Create(typeof(IService687), typeof(Service687)),
                Tuple.Create(typeof(IService688), typeof(Service688)),
                Tuple.Create(typeof(IService689), typeof(Service689)),
                Tuple.Create(typeof(IService690), typeof(Service690)),
                Tuple.Create(typeof(IService691), typeof(Service691)),
                Tuple.Create(typeof(IService692), typeof(Service692)),
                Tuple.Create(typeof(IService693), typeof(Service693)),
                Tuple.Create(typeof(IService694), typeof(Service694)),
                Tuple.Create(typeof(IService695), typeof(Service695)),
                Tuple.Create(typeof(IService696), typeof(Service696)),
                Tuple.Create(typeof(IService697), typeof(Service697)),
                Tuple.Create(typeof(IService698), typeof(Service698)),
                Tuple.Create(typeof(IService699), typeof(Service699)),
                Tuple.Create(typeof(IService700), typeof(Service700)),
                Tuple.Create(typeof(IService701), typeof(Service701)),
                Tuple.Create(typeof(IService702), typeof(Service702)),
                Tuple.Create(typeof(IService703), typeof(Service703)),
                Tuple.Create(typeof(IService704), typeof(Service704)),
                Tuple.Create(typeof(IService705), typeof(Service705)),
                Tuple.Create(typeof(IService706), typeof(Service706)),
                Tuple.Create(typeof(IService707), typeof(Service707)),
                Tuple.Create(typeof(IService708), typeof(Service708)),
                Tuple.Create(typeof(IService709), typeof(Service709)),
                Tuple.Create(typeof(IService710), typeof(Service710)),
                Tuple.Create(typeof(IService711), typeof(Service711)),
                Tuple.Create(typeof(IService712), typeof(Service712)),
                Tuple.Create(typeof(IService713), typeof(Service713)),
                Tuple.Create(typeof(IService714), typeof(Service714)),
                Tuple.Create(typeof(IService715), typeof(Service715)),
                Tuple.Create(typeof(IService716), typeof(Service716)),
                Tuple.Create(typeof(IService717), typeof(Service717)),
                Tuple.Create(typeof(IService718), typeof(Service718)),
                Tuple.Create(typeof(IService719), typeof(Service719)),
                Tuple.Create(typeof(IService720), typeof(Service720)),
                Tuple.Create(typeof(IService721), typeof(Service721)),
                Tuple.Create(typeof(IService722), typeof(Service722)),
                Tuple.Create(typeof(IService723), typeof(Service723)),
                Tuple.Create(typeof(IService724), typeof(Service724)),
                Tuple.Create(typeof(IService725), typeof(Service725)),
                Tuple.Create(typeof(IService726), typeof(Service726)),
                Tuple.Create(typeof(IService727), typeof(Service727)),
                Tuple.Create(typeof(IService728), typeof(Service728)),
                Tuple.Create(typeof(IService729), typeof(Service729)),
                Tuple.Create(typeof(IService730), typeof(Service730)),
                Tuple.Create(typeof(IService731), typeof(Service731)),
                Tuple.Create(typeof(IService732), typeof(Service732)),
                Tuple.Create(typeof(IService733), typeof(Service733)),
                Tuple.Create(typeof(IService734), typeof(Service734)),
                Tuple.Create(typeof(IService735), typeof(Service735)),
                Tuple.Create(typeof(IService736), typeof(Service736)),
                Tuple.Create(typeof(IService737), typeof(Service737)),
                Tuple.Create(typeof(IService738), typeof(Service738)),
                Tuple.Create(typeof(IService739), typeof(Service739)),
                Tuple.Create(typeof(IService740), typeof(Service740)),
                Tuple.Create(typeof(IService741), typeof(Service741)),
                Tuple.Create(typeof(IService742), typeof(Service742)),
                Tuple.Create(typeof(IService743), typeof(Service743)),
                Tuple.Create(typeof(IService744), typeof(Service744)),
                Tuple.Create(typeof(IService745), typeof(Service745)),
                Tuple.Create(typeof(IService746), typeof(Service746)),
                Tuple.Create(typeof(IService747), typeof(Service747)),
                Tuple.Create(typeof(IService748), typeof(Service748)),
                Tuple.Create(typeof(IService749), typeof(Service749)),
                Tuple.Create(typeof(IService750), typeof(Service750)),
                Tuple.Create(typeof(IService751), typeof(Service751)),
                Tuple.Create(typeof(IService752), typeof(Service752)),
                Tuple.Create(typeof(IService753), typeof(Service753)),
                Tuple.Create(typeof(IService754), typeof(Service754)),
                Tuple.Create(typeof(IService755), typeof(Service755)),
                Tuple.Create(typeof(IService756), typeof(Service756)),
                Tuple.Create(typeof(IService757), typeof(Service757)),
                Tuple.Create(typeof(IService758), typeof(Service758)),
                Tuple.Create(typeof(IService759), typeof(Service759)),
                Tuple.Create(typeof(IService760), typeof(Service760)),
                Tuple.Create(typeof(IService761), typeof(Service761)),
                Tuple.Create(typeof(IService762), typeof(Service762)),
                Tuple.Create(typeof(IService763), typeof(Service763)),
                Tuple.Create(typeof(IService764), typeof(Service764)),
                Tuple.Create(typeof(IService765), typeof(Service765)),
                Tuple.Create(typeof(IService766), typeof(Service766)),
                Tuple.Create(typeof(IService767), typeof(Service767)),
                Tuple.Create(typeof(IService768), typeof(Service768)),
                Tuple.Create(typeof(IService769), typeof(Service769)),
                Tuple.Create(typeof(IService770), typeof(Service770)),
                Tuple.Create(typeof(IService771), typeof(Service771)),
                Tuple.Create(typeof(IService772), typeof(Service772)),
                Tuple.Create(typeof(IService773), typeof(Service773)),
                Tuple.Create(typeof(IService774), typeof(Service774)),
                Tuple.Create(typeof(IService775), typeof(Service775)),
                Tuple.Create(typeof(IService776), typeof(Service776)),
                Tuple.Create(typeof(IService777), typeof(Service777)),
                Tuple.Create(typeof(IService778), typeof(Service778)),
                Tuple.Create(typeof(IService779), typeof(Service779)),
                Tuple.Create(typeof(IService780), typeof(Service780)),
                Tuple.Create(typeof(IService781), typeof(Service781)),
                Tuple.Create(typeof(IService782), typeof(Service782)),
                Tuple.Create(typeof(IService783), typeof(Service783)),
                Tuple.Create(typeof(IService784), typeof(Service784)),
                Tuple.Create(typeof(IService785), typeof(Service785)),
                Tuple.Create(typeof(IService786), typeof(Service786)),
                Tuple.Create(typeof(IService787), typeof(Service787)),
                Tuple.Create(typeof(IService788), typeof(Service788)),
                Tuple.Create(typeof(IService789), typeof(Service789)),
                Tuple.Create(typeof(IService790), typeof(Service790)),
                Tuple.Create(typeof(IService791), typeof(Service791)),
                Tuple.Create(typeof(IService792), typeof(Service792)),
                Tuple.Create(typeof(IService793), typeof(Service793)),
                Tuple.Create(typeof(IService794), typeof(Service794)),
                Tuple.Create(typeof(IService795), typeof(Service795)),
                Tuple.Create(typeof(IService796), typeof(Service796)),
                Tuple.Create(typeof(IService797), typeof(Service797)),
                Tuple.Create(typeof(IService798), typeof(Service798)),
                Tuple.Create(typeof(IService799), typeof(Service799)),
                Tuple.Create(typeof(IService800), typeof(Service800)),
                Tuple.Create(typeof(IService801), typeof(Service801)),
                Tuple.Create(typeof(IService802), typeof(Service802)),
                Tuple.Create(typeof(IService803), typeof(Service803)),
                Tuple.Create(typeof(IService804), typeof(Service804)),
                Tuple.Create(typeof(IService805), typeof(Service805)),
                Tuple.Create(typeof(IService806), typeof(Service806)),
                Tuple.Create(typeof(IService807), typeof(Service807)),
                Tuple.Create(typeof(IService808), typeof(Service808)),
                Tuple.Create(typeof(IService809), typeof(Service809)),
                Tuple.Create(typeof(IService810), typeof(Service810)),
                Tuple.Create(typeof(IService811), typeof(Service811)),
                Tuple.Create(typeof(IService812), typeof(Service812)),
                Tuple.Create(typeof(IService813), typeof(Service813)),
                Tuple.Create(typeof(IService814), typeof(Service814)),
                Tuple.Create(typeof(IService815), typeof(Service815)),
                Tuple.Create(typeof(IService816), typeof(Service816)),
                Tuple.Create(typeof(IService817), typeof(Service817)),
                Tuple.Create(typeof(IService818), typeof(Service818)),
                Tuple.Create(typeof(IService819), typeof(Service819)),
                Tuple.Create(typeof(IService820), typeof(Service820)),
                Tuple.Create(typeof(IService821), typeof(Service821)),
                Tuple.Create(typeof(IService822), typeof(Service822)),
                Tuple.Create(typeof(IService823), typeof(Service823)),
                Tuple.Create(typeof(IService824), typeof(Service824)),
                Tuple.Create(typeof(IService825), typeof(Service825)),
                Tuple.Create(typeof(IService826), typeof(Service826)),
                Tuple.Create(typeof(IService827), typeof(Service827)),
                Tuple.Create(typeof(IService828), typeof(Service828)),
                Tuple.Create(typeof(IService829), typeof(Service829)),
                Tuple.Create(typeof(IService830), typeof(Service830)),
                Tuple.Create(typeof(IService831), typeof(Service831)),
                Tuple.Create(typeof(IService832), typeof(Service832)),
                Tuple.Create(typeof(IService833), typeof(Service833)),
                Tuple.Create(typeof(IService834), typeof(Service834)),
                Tuple.Create(typeof(IService835), typeof(Service835)),
                Tuple.Create(typeof(IService836), typeof(Service836)),
                Tuple.Create(typeof(IService837), typeof(Service837)),
                Tuple.Create(typeof(IService838), typeof(Service838)),
                Tuple.Create(typeof(IService839), typeof(Service839)),
                Tuple.Create(typeof(IService840), typeof(Service840)),
                Tuple.Create(typeof(IService841), typeof(Service841)),
                Tuple.Create(typeof(IService842), typeof(Service842)),
                Tuple.Create(typeof(IService843), typeof(Service843)),
                Tuple.Create(typeof(IService844), typeof(Service844)),
                Tuple.Create(typeof(IService845), typeof(Service845)),
                Tuple.Create(typeof(IService846), typeof(Service846)),
                Tuple.Create(typeof(IService847), typeof(Service847)),
                Tuple.Create(typeof(IService848), typeof(Service848)),
                Tuple.Create(typeof(IService849), typeof(Service849)),
                Tuple.Create(typeof(IService850), typeof(Service850)),
                Tuple.Create(typeof(IService851), typeof(Service851)),
                Tuple.Create(typeof(IService852), typeof(Service852)),
                Tuple.Create(typeof(IService853), typeof(Service853)),
                Tuple.Create(typeof(IService854), typeof(Service854)),
                Tuple.Create(typeof(IService855), typeof(Service855)),
                Tuple.Create(typeof(IService856), typeof(Service856)),
                Tuple.Create(typeof(IService857), typeof(Service857)),
                Tuple.Create(typeof(IService858), typeof(Service858)),
                Tuple.Create(typeof(IService859), typeof(Service859)),
                Tuple.Create(typeof(IService860), typeof(Service860)),
                Tuple.Create(typeof(IService861), typeof(Service861)),
                Tuple.Create(typeof(IService862), typeof(Service862)),
                Tuple.Create(typeof(IService863), typeof(Service863)),
                Tuple.Create(typeof(IService864), typeof(Service864)),
                Tuple.Create(typeof(IService865), typeof(Service865)),
                Tuple.Create(typeof(IService866), typeof(Service866)),
                Tuple.Create(typeof(IService867), typeof(Service867)),
                Tuple.Create(typeof(IService868), typeof(Service868)),
                Tuple.Create(typeof(IService869), typeof(Service869)),
                Tuple.Create(typeof(IService870), typeof(Service870)),
                Tuple.Create(typeof(IService871), typeof(Service871)),
                Tuple.Create(typeof(IService872), typeof(Service872)),
                Tuple.Create(typeof(IService873), typeof(Service873)),
                Tuple.Create(typeof(IService874), typeof(Service874)),
                Tuple.Create(typeof(IService875), typeof(Service875)),
                Tuple.Create(typeof(IService876), typeof(Service876)),
                Tuple.Create(typeof(IService877), typeof(Service877)),
                Tuple.Create(typeof(IService878), typeof(Service878)),
                Tuple.Create(typeof(IService879), typeof(Service879)),
                Tuple.Create(typeof(IService880), typeof(Service880)),
                Tuple.Create(typeof(IService881), typeof(Service881)),
                Tuple.Create(typeof(IService882), typeof(Service882)),
                Tuple.Create(typeof(IService883), typeof(Service883)),
                Tuple.Create(typeof(IService884), typeof(Service884)),
                Tuple.Create(typeof(IService885), typeof(Service885)),
                Tuple.Create(typeof(IService886), typeof(Service886)),
                Tuple.Create(typeof(IService887), typeof(Service887)),
                Tuple.Create(typeof(IService888), typeof(Service888)),
                Tuple.Create(typeof(IService889), typeof(Service889)),
                Tuple.Create(typeof(IService890), typeof(Service890)),
                Tuple.Create(typeof(IService891), typeof(Service891)),
                Tuple.Create(typeof(IService892), typeof(Service892)),
                Tuple.Create(typeof(IService893), typeof(Service893)),
                Tuple.Create(typeof(IService894), typeof(Service894)),
                Tuple.Create(typeof(IService895), typeof(Service895)),
                Tuple.Create(typeof(IService896), typeof(Service896)),
                Tuple.Create(typeof(IService897), typeof(Service897)),
                Tuple.Create(typeof(IService898), typeof(Service898)),
                Tuple.Create(typeof(IService899), typeof(Service899)),
                Tuple.Create(typeof(IService900), typeof(Service900)),
                Tuple.Create(typeof(IService901), typeof(Service901)),
                Tuple.Create(typeof(IService902), typeof(Service902)),
                Tuple.Create(typeof(IService903), typeof(Service903)),
                Tuple.Create(typeof(IService904), typeof(Service904)),
                Tuple.Create(typeof(IService905), typeof(Service905)),
                Tuple.Create(typeof(IService906), typeof(Service906)),
                Tuple.Create(typeof(IService907), typeof(Service907)),
                Tuple.Create(typeof(IService908), typeof(Service908)),
                Tuple.Create(typeof(IService909), typeof(Service909)),
                Tuple.Create(typeof(IService910), typeof(Service910)),
                Tuple.Create(typeof(IService911), typeof(Service911)),
                Tuple.Create(typeof(IService912), typeof(Service912)),
                Tuple.Create(typeof(IService913), typeof(Service913)),
                Tuple.Create(typeof(IService914), typeof(Service914)),
                Tuple.Create(typeof(IService915), typeof(Service915)),
                Tuple.Create(typeof(IService916), typeof(Service916)),
                Tuple.Create(typeof(IService917), typeof(Service917)),
                Tuple.Create(typeof(IService918), typeof(Service918)),
                Tuple.Create(typeof(IService919), typeof(Service919)),
                Tuple.Create(typeof(IService920), typeof(Service920)),
                Tuple.Create(typeof(IService921), typeof(Service921)),
                Tuple.Create(typeof(IService922), typeof(Service922)),
                Tuple.Create(typeof(IService923), typeof(Service923)),
                Tuple.Create(typeof(IService924), typeof(Service924)),
                Tuple.Create(typeof(IService925), typeof(Service925)),
                Tuple.Create(typeof(IService926), typeof(Service926)),
                Tuple.Create(typeof(IService927), typeof(Service927)),
                Tuple.Create(typeof(IService928), typeof(Service928)),
                Tuple.Create(typeof(IService929), typeof(Service929)),
                Tuple.Create(typeof(IService930), typeof(Service930)),
                Tuple.Create(typeof(IService931), typeof(Service931)),
                Tuple.Create(typeof(IService932), typeof(Service932)),
                Tuple.Create(typeof(IService933), typeof(Service933)),
                Tuple.Create(typeof(IService934), typeof(Service934)),
                Tuple.Create(typeof(IService935), typeof(Service935)),
                Tuple.Create(typeof(IService936), typeof(Service936)),
                Tuple.Create(typeof(IService937), typeof(Service937)),
                Tuple.Create(typeof(IService938), typeof(Service938)),
                Tuple.Create(typeof(IService939), typeof(Service939)),
                Tuple.Create(typeof(IService940), typeof(Service940)),
                Tuple.Create(typeof(IService941), typeof(Service941)),
                Tuple.Create(typeof(IService942), typeof(Service942)),
                Tuple.Create(typeof(IService943), typeof(Service943)),
                Tuple.Create(typeof(IService944), typeof(Service944)),
                Tuple.Create(typeof(IService945), typeof(Service945)),
                Tuple.Create(typeof(IService946), typeof(Service946)),
                Tuple.Create(typeof(IService947), typeof(Service947)),
                Tuple.Create(typeof(IService948), typeof(Service948)),
                Tuple.Create(typeof(IService949), typeof(Service949)),
                Tuple.Create(typeof(IService950), typeof(Service950)),
                Tuple.Create(typeof(IService951), typeof(Service951)),
                Tuple.Create(typeof(IService952), typeof(Service952)),
                Tuple.Create(typeof(IService953), typeof(Service953)),
                Tuple.Create(typeof(IService954), typeof(Service954)),
                Tuple.Create(typeof(IService955), typeof(Service955)),
                Tuple.Create(typeof(IService956), typeof(Service956)),
                Tuple.Create(typeof(IService957), typeof(Service957)),
                Tuple.Create(typeof(IService958), typeof(Service958)),
                Tuple.Create(typeof(IService959), typeof(Service959)),
                Tuple.Create(typeof(IService960), typeof(Service960)),
                Tuple.Create(typeof(IService961), typeof(Service961)),
                Tuple.Create(typeof(IService962), typeof(Service962)),
                Tuple.Create(typeof(IService963), typeof(Service963)),
                Tuple.Create(typeof(IService964), typeof(Service964)),
                Tuple.Create(typeof(IService965), typeof(Service965)),
                Tuple.Create(typeof(IService966), typeof(Service966)),
                Tuple.Create(typeof(IService967), typeof(Service967)),
                Tuple.Create(typeof(IService968), typeof(Service968)),
                Tuple.Create(typeof(IService969), typeof(Service969)),
                Tuple.Create(typeof(IService970), typeof(Service970)),
                Tuple.Create(typeof(IService971), typeof(Service971)),
                Tuple.Create(typeof(IService972), typeof(Service972)),
                Tuple.Create(typeof(IService973), typeof(Service973)),
                Tuple.Create(typeof(IService974), typeof(Service974)),
                Tuple.Create(typeof(IService975), typeof(Service975)),
                Tuple.Create(typeof(IService976), typeof(Service976)),
                Tuple.Create(typeof(IService977), typeof(Service977)),
                Tuple.Create(typeof(IService978), typeof(Service978)),
                Tuple.Create(typeof(IService979), typeof(Service979)),
                Tuple.Create(typeof(IService980), typeof(Service980)),
                Tuple.Create(typeof(IService981), typeof(Service981)),
                Tuple.Create(typeof(IService982), typeof(Service982)),
                Tuple.Create(typeof(IService983), typeof(Service983)),
                Tuple.Create(typeof(IService984), typeof(Service984)),
                Tuple.Create(typeof(IService985), typeof(Service985)),
                Tuple.Create(typeof(IService986), typeof(Service986)),
                Tuple.Create(typeof(IService987), typeof(Service987)),
                Tuple.Create(typeof(IService988), typeof(Service988)),
                Tuple.Create(typeof(IService989), typeof(Service989)),
                Tuple.Create(typeof(IService990), typeof(Service990)),
                Tuple.Create(typeof(IService991), typeof(Service991)),
                Tuple.Create(typeof(IService992), typeof(Service992)),
                Tuple.Create(typeof(IService993), typeof(Service993)),
                Tuple.Create(typeof(IService994), typeof(Service994)),
                Tuple.Create(typeof(IService995), typeof(Service995)),
                Tuple.Create(typeof(IService996), typeof(Service996)),
                Tuple.Create(typeof(IService997), typeof(Service997)),
                Tuple.Create(typeof(IService998), typeof(Service998)),
                Tuple.Create(typeof(IService999), typeof(Service999)),
            };
            TypesMap = typesMap;
        }

        public static Tuple<Type, Type>[] TypesMap { get; }

        private interface IService0 { }
        private class Service0 : IService0 { }
        private interface IService1 { }
        private class Service1 : IService1 { }
        private interface IService2 { }
        private class Service2 : IService2 { }
        private interface IService3 { }
        private class Service3 : IService3 { }
        private interface IService4 { }
        private class Service4 : IService4 { }
        private interface IService5 { }
        private class Service5 : IService5 { }
        private interface IService6 { }
        private class Service6 : IService6 { }
        private interface IService7 { }
        private class Service7 : IService7 { }
        private interface IService8 { }
        private class Service8 : IService8 { }
        private interface IService9 { }
        private class Service9 : IService9 { }
        private interface IService10 { }
        private class Service10 : IService10 { }
        private interface IService11 { }
        private class Service11 : IService11 { }
        private interface IService12 { }
        private class Service12 : IService12 { }
        private interface IService13 { }
        private class Service13 : IService13 { }
        private interface IService14 { }
        private class Service14 : IService14 { }
        private interface IService15 { }
        private class Service15 : IService15 { }
        private interface IService16 { }
        private class Service16 : IService16 { }
        private interface IService17 { }
        private class Service17 : IService17 { }
        private interface IService18 { }
        private class Service18 : IService18 { }
        private interface IService19 { }
        private class Service19 : IService19 { }
        private interface IService20 { }
        private class Service20 : IService20 { }
        private interface IService21 { }
        private class Service21 : IService21 { }
        private interface IService22 { }
        private class Service22 : IService22 { }
        private interface IService23 { }
        private class Service23 : IService23 { }
        private interface IService24 { }
        private class Service24 : IService24 { }
        private interface IService25 { }
        private class Service25 : IService25 { }
        private interface IService26 { }
        private class Service26 : IService26 { }
        private interface IService27 { }
        private class Service27 : IService27 { }
        private interface IService28 { }
        private class Service28 : IService28 { }
        private interface IService29 { }
        private class Service29 : IService29 { }
        private interface IService30 { }
        private class Service30 : IService30 { }
        private interface IService31 { }
        private class Service31 : IService31 { }
        private interface IService32 { }
        private class Service32 : IService32 { }
        private interface IService33 { }
        private class Service33 : IService33 { }
        private interface IService34 { }
        private class Service34 : IService34 { }
        private interface IService35 { }
        private class Service35 : IService35 { }
        private interface IService36 { }
        private class Service36 : IService36 { }
        private interface IService37 { }
        private class Service37 : IService37 { }
        private interface IService38 { }
        private class Service38 : IService38 { }
        private interface IService39 { }
        private class Service39 : IService39 { }
        private interface IService40 { }
        private class Service40 : IService40 { }
        private interface IService41 { }
        private class Service41 : IService41 { }
        private interface IService42 { }
        private class Service42 : IService42 { }
        private interface IService43 { }
        private class Service43 : IService43 { }
        private interface IService44 { }
        private class Service44 : IService44 { }
        private interface IService45 { }
        private class Service45 : IService45 { }
        private interface IService46 { }
        private class Service46 : IService46 { }
        private interface IService47 { }
        private class Service47 : IService47 { }
        private interface IService48 { }
        private class Service48 : IService48 { }
        private interface IService49 { }
        private class Service49 : IService49 { }
        private interface IService50 { }
        private class Service50 : IService50 { }
        private interface IService51 { }
        private class Service51 : IService51 { }
        private interface IService52 { }
        private class Service52 : IService52 { }
        private interface IService53 { }
        private class Service53 : IService53 { }
        private interface IService54 { }
        private class Service54 : IService54 { }
        private interface IService55 { }
        private class Service55 : IService55 { }
        private interface IService56 { }
        private class Service56 : IService56 { }
        private interface IService57 { }
        private class Service57 : IService57 { }
        private interface IService58 { }
        private class Service58 : IService58 { }
        private interface IService59 { }
        private class Service59 : IService59 { }
        private interface IService60 { }
        private class Service60 : IService60 { }
        private interface IService61 { }
        private class Service61 : IService61 { }
        private interface IService62 { }
        private class Service62 : IService62 { }
        private interface IService63 { }
        private class Service63 : IService63 { }
        private interface IService64 { }
        private class Service64 : IService64 { }
        private interface IService65 { }
        private class Service65 : IService65 { }
        private interface IService66 { }
        private class Service66 : IService66 { }
        private interface IService67 { }
        private class Service67 : IService67 { }
        private interface IService68 { }
        private class Service68 : IService68 { }
        private interface IService69 { }
        private class Service69 : IService69 { }
        private interface IService70 { }
        private class Service70 : IService70 { }
        private interface IService71 { }
        private class Service71 : IService71 { }
        private interface IService72 { }
        private class Service72 : IService72 { }
        private interface IService73 { }
        private class Service73 : IService73 { }
        private interface IService74 { }
        private class Service74 : IService74 { }
        private interface IService75 { }
        private class Service75 : IService75 { }
        private interface IService76 { }
        private class Service76 : IService76 { }
        private interface IService77 { }
        private class Service77 : IService77 { }
        private interface IService78 { }
        private class Service78 : IService78 { }
        private interface IService79 { }
        private class Service79 : IService79 { }
        private interface IService80 { }
        private class Service80 : IService80 { }
        private interface IService81 { }
        private class Service81 : IService81 { }
        private interface IService82 { }
        private class Service82 : IService82 { }
        private interface IService83 { }
        private class Service83 : IService83 { }
        private interface IService84 { }
        private class Service84 : IService84 { }
        private interface IService85 { }
        private class Service85 : IService85 { }
        private interface IService86 { }
        private class Service86 : IService86 { }
        private interface IService87 { }
        private class Service87 : IService87 { }
        private interface IService88 { }
        private class Service88 : IService88 { }
        private interface IService89 { }
        private class Service89 : IService89 { }
        private interface IService90 { }
        private class Service90 : IService90 { }
        private interface IService91 { }
        private class Service91 : IService91 { }
        private interface IService92 { }
        private class Service92 : IService92 { }
        private interface IService93 { }
        private class Service93 : IService93 { }
        private interface IService94 { }
        private class Service94 : IService94 { }
        private interface IService95 { }
        private class Service95 : IService95 { }
        private interface IService96 { }
        private class Service96 : IService96 { }
        private interface IService97 { }
        private class Service97 : IService97 { }
        private interface IService98 { }
        private class Service98 : IService98 { }
        private interface IService99 { }
        private class Service99 : IService99 { }
        private interface IService100 { }
        private class Service100 : IService100 { }
        private interface IService101 { }
        private class Service101 : IService101 { }
        private interface IService102 { }
        private class Service102 : IService102 { }
        private interface IService103 { }
        private class Service103 : IService103 { }
        private interface IService104 { }
        private class Service104 : IService104 { }
        private interface IService105 { }
        private class Service105 : IService105 { }
        private interface IService106 { }
        private class Service106 : IService106 { }
        private interface IService107 { }
        private class Service107 : IService107 { }
        private interface IService108 { }
        private class Service108 : IService108 { }
        private interface IService109 { }
        private class Service109 : IService109 { }
        private interface IService110 { }
        private class Service110 : IService110 { }
        private interface IService111 { }
        private class Service111 : IService111 { }
        private interface IService112 { }
        private class Service112 : IService112 { }
        private interface IService113 { }
        private class Service113 : IService113 { }
        private interface IService114 { }
        private class Service114 : IService114 { }
        private interface IService115 { }
        private class Service115 : IService115 { }
        private interface IService116 { }
        private class Service116 : IService116 { }
        private interface IService117 { }
        private class Service117 : IService117 { }
        private interface IService118 { }
        private class Service118 : IService118 { }
        private interface IService119 { }
        private class Service119 : IService119 { }
        private interface IService120 { }
        private class Service120 : IService120 { }
        private interface IService121 { }
        private class Service121 : IService121 { }
        private interface IService122 { }
        private class Service122 : IService122 { }
        private interface IService123 { }
        private class Service123 : IService123 { }
        private interface IService124 { }
        private class Service124 : IService124 { }
        private interface IService125 { }
        private class Service125 : IService125 { }
        private interface IService126 { }
        private class Service126 : IService126 { }
        private interface IService127 { }
        private class Service127 : IService127 { }
        private interface IService128 { }
        private class Service128 : IService128 { }
        private interface IService129 { }
        private class Service129 : IService129 { }
        private interface IService130 { }
        private class Service130 : IService130 { }
        private interface IService131 { }
        private class Service131 : IService131 { }
        private interface IService132 { }
        private class Service132 : IService132 { }
        private interface IService133 { }
        private class Service133 : IService133 { }
        private interface IService134 { }
        private class Service134 : IService134 { }
        private interface IService135 { }
        private class Service135 : IService135 { }
        private interface IService136 { }
        private class Service136 : IService136 { }
        private interface IService137 { }
        private class Service137 : IService137 { }
        private interface IService138 { }
        private class Service138 : IService138 { }
        private interface IService139 { }
        private class Service139 : IService139 { }
        private interface IService140 { }
        private class Service140 : IService140 { }
        private interface IService141 { }
        private class Service141 : IService141 { }
        private interface IService142 { }
        private class Service142 : IService142 { }
        private interface IService143 { }
        private class Service143 : IService143 { }
        private interface IService144 { }
        private class Service144 : IService144 { }
        private interface IService145 { }
        private class Service145 : IService145 { }
        private interface IService146 { }
        private class Service146 : IService146 { }
        private interface IService147 { }
        private class Service147 : IService147 { }
        private interface IService148 { }
        private class Service148 : IService148 { }
        private interface IService149 { }
        private class Service149 : IService149 { }
        private interface IService150 { }
        private class Service150 : IService150 { }
        private interface IService151 { }
        private class Service151 : IService151 { }
        private interface IService152 { }
        private class Service152 : IService152 { }
        private interface IService153 { }
        private class Service153 : IService153 { }
        private interface IService154 { }
        private class Service154 : IService154 { }
        private interface IService155 { }
        private class Service155 : IService155 { }
        private interface IService156 { }
        private class Service156 : IService156 { }
        private interface IService157 { }
        private class Service157 : IService157 { }
        private interface IService158 { }
        private class Service158 : IService158 { }
        private interface IService159 { }
        private class Service159 : IService159 { }
        private interface IService160 { }
        private class Service160 : IService160 { }
        private interface IService161 { }
        private class Service161 : IService161 { }
        private interface IService162 { }
        private class Service162 : IService162 { }
        private interface IService163 { }
        private class Service163 : IService163 { }
        private interface IService164 { }
        private class Service164 : IService164 { }
        private interface IService165 { }
        private class Service165 : IService165 { }
        private interface IService166 { }
        private class Service166 : IService166 { }
        private interface IService167 { }
        private class Service167 : IService167 { }
        private interface IService168 { }
        private class Service168 : IService168 { }
        private interface IService169 { }
        private class Service169 : IService169 { }
        private interface IService170 { }
        private class Service170 : IService170 { }
        private interface IService171 { }
        private class Service171 : IService171 { }
        private interface IService172 { }
        private class Service172 : IService172 { }
        private interface IService173 { }
        private class Service173 : IService173 { }
        private interface IService174 { }
        private class Service174 : IService174 { }
        private interface IService175 { }
        private class Service175 : IService175 { }
        private interface IService176 { }
        private class Service176 : IService176 { }
        private interface IService177 { }
        private class Service177 : IService177 { }
        private interface IService178 { }
        private class Service178 : IService178 { }
        private interface IService179 { }
        private class Service179 : IService179 { }
        private interface IService180 { }
        private class Service180 : IService180 { }
        private interface IService181 { }
        private class Service181 : IService181 { }
        private interface IService182 { }
        private class Service182 : IService182 { }
        private interface IService183 { }
        private class Service183 : IService183 { }
        private interface IService184 { }
        private class Service184 : IService184 { }
        private interface IService185 { }
        private class Service185 : IService185 { }
        private interface IService186 { }
        private class Service186 : IService186 { }
        private interface IService187 { }
        private class Service187 : IService187 { }
        private interface IService188 { }
        private class Service188 : IService188 { }
        private interface IService189 { }
        private class Service189 : IService189 { }
        private interface IService190 { }
        private class Service190 : IService190 { }
        private interface IService191 { }
        private class Service191 : IService191 { }
        private interface IService192 { }
        private class Service192 : IService192 { }
        private interface IService193 { }
        private class Service193 : IService193 { }
        private interface IService194 { }
        private class Service194 : IService194 { }
        private interface IService195 { }
        private class Service195 : IService195 { }
        private interface IService196 { }
        private class Service196 : IService196 { }
        private interface IService197 { }
        private class Service197 : IService197 { }
        private interface IService198 { }
        private class Service198 : IService198 { }
        private interface IService199 { }
        private class Service199 : IService199 { }
        private interface IService200 { }
        private class Service200 : IService200 { }
        private interface IService201 { }
        private class Service201 : IService201 { }
        private interface IService202 { }
        private class Service202 : IService202 { }
        private interface IService203 { }
        private class Service203 : IService203 { }
        private interface IService204 { }
        private class Service204 : IService204 { }
        private interface IService205 { }
        private class Service205 : IService205 { }
        private interface IService206 { }
        private class Service206 : IService206 { }
        private interface IService207 { }
        private class Service207 : IService207 { }
        private interface IService208 { }
        private class Service208 : IService208 { }
        private interface IService209 { }
        private class Service209 : IService209 { }
        private interface IService210 { }
        private class Service210 : IService210 { }
        private interface IService211 { }
        private class Service211 : IService211 { }
        private interface IService212 { }
        private class Service212 : IService212 { }
        private interface IService213 { }
        private class Service213 : IService213 { }
        private interface IService214 { }
        private class Service214 : IService214 { }
        private interface IService215 { }
        private class Service215 : IService215 { }
        private interface IService216 { }
        private class Service216 : IService216 { }
        private interface IService217 { }
        private class Service217 : IService217 { }
        private interface IService218 { }
        private class Service218 : IService218 { }
        private interface IService219 { }
        private class Service219 : IService219 { }
        private interface IService220 { }
        private class Service220 : IService220 { }
        private interface IService221 { }
        private class Service221 : IService221 { }
        private interface IService222 { }
        private class Service222 : IService222 { }
        private interface IService223 { }
        private class Service223 : IService223 { }
        private interface IService224 { }
        private class Service224 : IService224 { }
        private interface IService225 { }
        private class Service225 : IService225 { }
        private interface IService226 { }
        private class Service226 : IService226 { }
        private interface IService227 { }
        private class Service227 : IService227 { }
        private interface IService228 { }
        private class Service228 : IService228 { }
        private interface IService229 { }
        private class Service229 : IService229 { }
        private interface IService230 { }
        private class Service230 : IService230 { }
        private interface IService231 { }
        private class Service231 : IService231 { }
        private interface IService232 { }
        private class Service232 : IService232 { }
        private interface IService233 { }
        private class Service233 : IService233 { }
        private interface IService234 { }
        private class Service234 : IService234 { }
        private interface IService235 { }
        private class Service235 : IService235 { }
        private interface IService236 { }
        private class Service236 : IService236 { }
        private interface IService237 { }
        private class Service237 : IService237 { }
        private interface IService238 { }
        private class Service238 : IService238 { }
        private interface IService239 { }
        private class Service239 : IService239 { }
        private interface IService240 { }
        private class Service240 : IService240 { }
        private interface IService241 { }
        private class Service241 : IService241 { }
        private interface IService242 { }
        private class Service242 : IService242 { }
        private interface IService243 { }
        private class Service243 : IService243 { }
        private interface IService244 { }
        private class Service244 : IService244 { }
        private interface IService245 { }
        private class Service245 : IService245 { }
        private interface IService246 { }
        private class Service246 : IService246 { }
        private interface IService247 { }
        private class Service247 : IService247 { }
        private interface IService248 { }
        private class Service248 : IService248 { }
        private interface IService249 { }
        private class Service249 : IService249 { }
        private interface IService250 { }
        private class Service250 : IService250 { }
        private interface IService251 { }
        private class Service251 : IService251 { }
        private interface IService252 { }
        private class Service252 : IService252 { }
        private interface IService253 { }
        private class Service253 : IService253 { }
        private interface IService254 { }
        private class Service254 : IService254 { }
        private interface IService255 { }
        private class Service255 : IService255 { }
        private interface IService256 { }
        private class Service256 : IService256 { }
        private interface IService257 { }
        private class Service257 : IService257 { }
        private interface IService258 { }
        private class Service258 : IService258 { }
        private interface IService259 { }
        private class Service259 : IService259 { }
        private interface IService260 { }
        private class Service260 : IService260 { }
        private interface IService261 { }
        private class Service261 : IService261 { }
        private interface IService262 { }
        private class Service262 : IService262 { }
        private interface IService263 { }
        private class Service263 : IService263 { }
        private interface IService264 { }
        private class Service264 : IService264 { }
        private interface IService265 { }
        private class Service265 : IService265 { }
        private interface IService266 { }
        private class Service266 : IService266 { }
        private interface IService267 { }
        private class Service267 : IService267 { }
        private interface IService268 { }
        private class Service268 : IService268 { }
        private interface IService269 { }
        private class Service269 : IService269 { }
        private interface IService270 { }
        private class Service270 : IService270 { }
        private interface IService271 { }
        private class Service271 : IService271 { }
        private interface IService272 { }
        private class Service272 : IService272 { }
        private interface IService273 { }
        private class Service273 : IService273 { }
        private interface IService274 { }
        private class Service274 : IService274 { }
        private interface IService275 { }
        private class Service275 : IService275 { }
        private interface IService276 { }
        private class Service276 : IService276 { }
        private interface IService277 { }
        private class Service277 : IService277 { }
        private interface IService278 { }
        private class Service278 : IService278 { }
        private interface IService279 { }
        private class Service279 : IService279 { }
        private interface IService280 { }
        private class Service280 : IService280 { }
        private interface IService281 { }
        private class Service281 : IService281 { }
        private interface IService282 { }
        private class Service282 : IService282 { }
        private interface IService283 { }
        private class Service283 : IService283 { }
        private interface IService284 { }
        private class Service284 : IService284 { }
        private interface IService285 { }
        private class Service285 : IService285 { }
        private interface IService286 { }
        private class Service286 : IService286 { }
        private interface IService287 { }
        private class Service287 : IService287 { }
        private interface IService288 { }
        private class Service288 : IService288 { }
        private interface IService289 { }
        private class Service289 : IService289 { }
        private interface IService290 { }
        private class Service290 : IService290 { }
        private interface IService291 { }
        private class Service291 : IService291 { }
        private interface IService292 { }
        private class Service292 : IService292 { }
        private interface IService293 { }
        private class Service293 : IService293 { }
        private interface IService294 { }
        private class Service294 : IService294 { }
        private interface IService295 { }
        private class Service295 : IService295 { }
        private interface IService296 { }
        private class Service296 : IService296 { }
        private interface IService297 { }
        private class Service297 : IService297 { }
        private interface IService298 { }
        private class Service298 : IService298 { }
        private interface IService299 { }
        private class Service299 : IService299 { }
        private interface IService300 { }
        private class Service300 : IService300 { }
        private interface IService301 { }
        private class Service301 : IService301 { }
        private interface IService302 { }
        private class Service302 : IService302 { }
        private interface IService303 { }
        private class Service303 : IService303 { }
        private interface IService304 { }
        private class Service304 : IService304 { }
        private interface IService305 { }
        private class Service305 : IService305 { }
        private interface IService306 { }
        private class Service306 : IService306 { }
        private interface IService307 { }
        private class Service307 : IService307 { }
        private interface IService308 { }
        private class Service308 : IService308 { }
        private interface IService309 { }
        private class Service309 : IService309 { }
        private interface IService310 { }
        private class Service310 : IService310 { }
        private interface IService311 { }
        private class Service311 : IService311 { }
        private interface IService312 { }
        private class Service312 : IService312 { }
        private interface IService313 { }
        private class Service313 : IService313 { }
        private interface IService314 { }
        private class Service314 : IService314 { }
        private interface IService315 { }
        private class Service315 : IService315 { }
        private interface IService316 { }
        private class Service316 : IService316 { }
        private interface IService317 { }
        private class Service317 : IService317 { }
        private interface IService318 { }
        private class Service318 : IService318 { }
        private interface IService319 { }
        private class Service319 : IService319 { }
        private interface IService320 { }
        private class Service320 : IService320 { }
        private interface IService321 { }
        private class Service321 : IService321 { }
        private interface IService322 { }
        private class Service322 : IService322 { }
        private interface IService323 { }
        private class Service323 : IService323 { }
        private interface IService324 { }
        private class Service324 : IService324 { }
        private interface IService325 { }
        private class Service325 : IService325 { }
        private interface IService326 { }
        private class Service326 : IService326 { }
        private interface IService327 { }
        private class Service327 : IService327 { }
        private interface IService328 { }
        private class Service328 : IService328 { }
        private interface IService329 { }
        private class Service329 : IService329 { }
        private interface IService330 { }
        private class Service330 : IService330 { }
        private interface IService331 { }
        private class Service331 : IService331 { }
        private interface IService332 { }
        private class Service332 : IService332 { }
        private interface IService333 { }
        private class Service333 : IService333 { }
        private interface IService334 { }
        private class Service334 : IService334 { }
        private interface IService335 { }
        private class Service335 : IService335 { }
        private interface IService336 { }
        private class Service336 : IService336 { }
        private interface IService337 { }
        private class Service337 : IService337 { }
        private interface IService338 { }
        private class Service338 : IService338 { }
        private interface IService339 { }
        private class Service339 : IService339 { }
        private interface IService340 { }
        private class Service340 : IService340 { }
        private interface IService341 { }
        private class Service341 : IService341 { }
        private interface IService342 { }
        private class Service342 : IService342 { }
        private interface IService343 { }
        private class Service343 : IService343 { }
        private interface IService344 { }
        private class Service344 : IService344 { }
        private interface IService345 { }
        private class Service345 : IService345 { }
        private interface IService346 { }
        private class Service346 : IService346 { }
        private interface IService347 { }
        private class Service347 : IService347 { }
        private interface IService348 { }
        private class Service348 : IService348 { }
        private interface IService349 { }
        private class Service349 : IService349 { }
        private interface IService350 { }
        private class Service350 : IService350 { }
        private interface IService351 { }
        private class Service351 : IService351 { }
        private interface IService352 { }
        private class Service352 : IService352 { }
        private interface IService353 { }
        private class Service353 : IService353 { }
        private interface IService354 { }
        private class Service354 : IService354 { }
        private interface IService355 { }
        private class Service355 : IService355 { }
        private interface IService356 { }
        private class Service356 : IService356 { }
        private interface IService357 { }
        private class Service357 : IService357 { }
        private interface IService358 { }
        private class Service358 : IService358 { }
        private interface IService359 { }
        private class Service359 : IService359 { }
        private interface IService360 { }
        private class Service360 : IService360 { }
        private interface IService361 { }
        private class Service361 : IService361 { }
        private interface IService362 { }
        private class Service362 : IService362 { }
        private interface IService363 { }
        private class Service363 : IService363 { }
        private interface IService364 { }
        private class Service364 : IService364 { }
        private interface IService365 { }
        private class Service365 : IService365 { }
        private interface IService366 { }
        private class Service366 : IService366 { }
        private interface IService367 { }
        private class Service367 : IService367 { }
        private interface IService368 { }
        private class Service368 : IService368 { }
        private interface IService369 { }
        private class Service369 : IService369 { }
        private interface IService370 { }
        private class Service370 : IService370 { }
        private interface IService371 { }
        private class Service371 : IService371 { }
        private interface IService372 { }
        private class Service372 : IService372 { }
        private interface IService373 { }
        private class Service373 : IService373 { }
        private interface IService374 { }
        private class Service374 : IService374 { }
        private interface IService375 { }
        private class Service375 : IService375 { }
        private interface IService376 { }
        private class Service376 : IService376 { }
        private interface IService377 { }
        private class Service377 : IService377 { }
        private interface IService378 { }
        private class Service378 : IService378 { }
        private interface IService379 { }
        private class Service379 : IService379 { }
        private interface IService380 { }
        private class Service380 : IService380 { }
        private interface IService381 { }
        private class Service381 : IService381 { }
        private interface IService382 { }
        private class Service382 : IService382 { }
        private interface IService383 { }
        private class Service383 : IService383 { }
        private interface IService384 { }
        private class Service384 : IService384 { }
        private interface IService385 { }
        private class Service385 : IService385 { }
        private interface IService386 { }
        private class Service386 : IService386 { }
        private interface IService387 { }
        private class Service387 : IService387 { }
        private interface IService388 { }
        private class Service388 : IService388 { }
        private interface IService389 { }
        private class Service389 : IService389 { }
        private interface IService390 { }
        private class Service390 : IService390 { }
        private interface IService391 { }
        private class Service391 : IService391 { }
        private interface IService392 { }
        private class Service392 : IService392 { }
        private interface IService393 { }
        private class Service393 : IService393 { }
        private interface IService394 { }
        private class Service394 : IService394 { }
        private interface IService395 { }
        private class Service395 : IService395 { }
        private interface IService396 { }
        private class Service396 : IService396 { }
        private interface IService397 { }
        private class Service397 : IService397 { }
        private interface IService398 { }
        private class Service398 : IService398 { }
        private interface IService399 { }
        private class Service399 : IService399 { }
        private interface IService400 { }
        private class Service400 : IService400 { }
        private interface IService401 { }
        private class Service401 : IService401 { }
        private interface IService402 { }
        private class Service402 : IService402 { }
        private interface IService403 { }
        private class Service403 : IService403 { }
        private interface IService404 { }
        private class Service404 : IService404 { }
        private interface IService405 { }
        private class Service405 : IService405 { }
        private interface IService406 { }
        private class Service406 : IService406 { }
        private interface IService407 { }
        private class Service407 : IService407 { }
        private interface IService408 { }
        private class Service408 : IService408 { }
        private interface IService409 { }
        private class Service409 : IService409 { }
        private interface IService410 { }
        private class Service410 : IService410 { }
        private interface IService411 { }
        private class Service411 : IService411 { }
        private interface IService412 { }
        private class Service412 : IService412 { }
        private interface IService413 { }
        private class Service413 : IService413 { }
        private interface IService414 { }
        private class Service414 : IService414 { }
        private interface IService415 { }
        private class Service415 : IService415 { }
        private interface IService416 { }
        private class Service416 : IService416 { }
        private interface IService417 { }
        private class Service417 : IService417 { }
        private interface IService418 { }
        private class Service418 : IService418 { }
        private interface IService419 { }
        private class Service419 : IService419 { }
        private interface IService420 { }
        private class Service420 : IService420 { }
        private interface IService421 { }
        private class Service421 : IService421 { }
        private interface IService422 { }
        private class Service422 : IService422 { }
        private interface IService423 { }
        private class Service423 : IService423 { }
        private interface IService424 { }
        private class Service424 : IService424 { }
        private interface IService425 { }
        private class Service425 : IService425 { }
        private interface IService426 { }
        private class Service426 : IService426 { }
        private interface IService427 { }
        private class Service427 : IService427 { }
        private interface IService428 { }
        private class Service428 : IService428 { }
        private interface IService429 { }
        private class Service429 : IService429 { }
        private interface IService430 { }
        private class Service430 : IService430 { }
        private interface IService431 { }
        private class Service431 : IService431 { }
        private interface IService432 { }
        private class Service432 : IService432 { }
        private interface IService433 { }
        private class Service433 : IService433 { }
        private interface IService434 { }
        private class Service434 : IService434 { }
        private interface IService435 { }
        private class Service435 : IService435 { }
        private interface IService436 { }
        private class Service436 : IService436 { }
        private interface IService437 { }
        private class Service437 : IService437 { }
        private interface IService438 { }
        private class Service438 : IService438 { }
        private interface IService439 { }
        private class Service439 : IService439 { }
        private interface IService440 { }
        private class Service440 : IService440 { }
        private interface IService441 { }
        private class Service441 : IService441 { }
        private interface IService442 { }
        private class Service442 : IService442 { }
        private interface IService443 { }
        private class Service443 : IService443 { }
        private interface IService444 { }
        private class Service444 : IService444 { }
        private interface IService445 { }
        private class Service445 : IService445 { }
        private interface IService446 { }
        private class Service446 : IService446 { }
        private interface IService447 { }
        private class Service447 : IService447 { }
        private interface IService448 { }
        private class Service448 : IService448 { }
        private interface IService449 { }
        private class Service449 : IService449 { }
        private interface IService450 { }
        private class Service450 : IService450 { }
        private interface IService451 { }
        private class Service451 : IService451 { }
        private interface IService452 { }
        private class Service452 : IService452 { }
        private interface IService453 { }
        private class Service453 : IService453 { }
        private interface IService454 { }
        private class Service454 : IService454 { }
        private interface IService455 { }
        private class Service455 : IService455 { }
        private interface IService456 { }
        private class Service456 : IService456 { }
        private interface IService457 { }
        private class Service457 : IService457 { }
        private interface IService458 { }
        private class Service458 : IService458 { }
        private interface IService459 { }
        private class Service459 : IService459 { }
        private interface IService460 { }
        private class Service460 : IService460 { }
        private interface IService461 { }
        private class Service461 : IService461 { }
        private interface IService462 { }
        private class Service462 : IService462 { }
        private interface IService463 { }
        private class Service463 : IService463 { }
        private interface IService464 { }
        private class Service464 : IService464 { }
        private interface IService465 { }
        private class Service465 : IService465 { }
        private interface IService466 { }
        private class Service466 : IService466 { }
        private interface IService467 { }
        private class Service467 : IService467 { }
        private interface IService468 { }
        private class Service468 : IService468 { }
        private interface IService469 { }
        private class Service469 : IService469 { }
        private interface IService470 { }
        private class Service470 : IService470 { }
        private interface IService471 { }
        private class Service471 : IService471 { }
        private interface IService472 { }
        private class Service472 : IService472 { }
        private interface IService473 { }
        private class Service473 : IService473 { }
        private interface IService474 { }
        private class Service474 : IService474 { }
        private interface IService475 { }
        private class Service475 : IService475 { }
        private interface IService476 { }
        private class Service476 : IService476 { }
        private interface IService477 { }
        private class Service477 : IService477 { }
        private interface IService478 { }
        private class Service478 : IService478 { }
        private interface IService479 { }
        private class Service479 : IService479 { }
        private interface IService480 { }
        private class Service480 : IService480 { }
        private interface IService481 { }
        private class Service481 : IService481 { }
        private interface IService482 { }
        private class Service482 : IService482 { }
        private interface IService483 { }
        private class Service483 : IService483 { }
        private interface IService484 { }
        private class Service484 : IService484 { }
        private interface IService485 { }
        private class Service485 : IService485 { }
        private interface IService486 { }
        private class Service486 : IService486 { }
        private interface IService487 { }
        private class Service487 : IService487 { }
        private interface IService488 { }
        private class Service488 : IService488 { }
        private interface IService489 { }
        private class Service489 : IService489 { }
        private interface IService490 { }
        private class Service490 : IService490 { }
        private interface IService491 { }
        private class Service491 : IService491 { }
        private interface IService492 { }
        private class Service492 : IService492 { }
        private interface IService493 { }
        private class Service493 : IService493 { }
        private interface IService494 { }
        private class Service494 : IService494 { }
        private interface IService495 { }
        private class Service495 : IService495 { }
        private interface IService496 { }
        private class Service496 : IService496 { }
        private interface IService497 { }
        private class Service497 : IService497 { }
        private interface IService498 { }
        private class Service498 : IService498 { }
        private interface IService499 { }
        private class Service499 : IService499 { }
        private interface IService500 { }
        private class Service500 : IService500 { }
        private interface IService501 { }
        private class Service501 : IService501 { }
        private interface IService502 { }
        private class Service502 : IService502 { }
        private interface IService503 { }
        private class Service503 : IService503 { }
        private interface IService504 { }
        private class Service504 : IService504 { }
        private interface IService505 { }
        private class Service505 : IService505 { }
        private interface IService506 { }
        private class Service506 : IService506 { }
        private interface IService507 { }
        private class Service507 : IService507 { }
        private interface IService508 { }
        private class Service508 : IService508 { }
        private interface IService509 { }
        private class Service509 : IService509 { }
        private interface IService510 { }
        private class Service510 : IService510 { }
        private interface IService511 { }
        private class Service511 : IService511 { }
        private interface IService512 { }
        private class Service512 : IService512 { }
        private interface IService513 { }
        private class Service513 : IService513 { }
        private interface IService514 { }
        private class Service514 : IService514 { }
        private interface IService515 { }
        private class Service515 : IService515 { }
        private interface IService516 { }
        private class Service516 : IService516 { }
        private interface IService517 { }
        private class Service517 : IService517 { }
        private interface IService518 { }
        private class Service518 : IService518 { }
        private interface IService519 { }
        private class Service519 : IService519 { }
        private interface IService520 { }
        private class Service520 : IService520 { }
        private interface IService521 { }
        private class Service521 : IService521 { }
        private interface IService522 { }
        private class Service522 : IService522 { }
        private interface IService523 { }
        private class Service523 : IService523 { }
        private interface IService524 { }
        private class Service524 : IService524 { }
        private interface IService525 { }
        private class Service525 : IService525 { }
        private interface IService526 { }
        private class Service526 : IService526 { }
        private interface IService527 { }
        private class Service527 : IService527 { }
        private interface IService528 { }
        private class Service528 : IService528 { }
        private interface IService529 { }
        private class Service529 : IService529 { }
        private interface IService530 { }
        private class Service530 : IService530 { }
        private interface IService531 { }
        private class Service531 : IService531 { }
        private interface IService532 { }
        private class Service532 : IService532 { }
        private interface IService533 { }
        private class Service533 : IService533 { }
        private interface IService534 { }
        private class Service534 : IService534 { }
        private interface IService535 { }
        private class Service535 : IService535 { }
        private interface IService536 { }
        private class Service536 : IService536 { }
        private interface IService537 { }
        private class Service537 : IService537 { }
        private interface IService538 { }
        private class Service538 : IService538 { }
        private interface IService539 { }
        private class Service539 : IService539 { }
        private interface IService540 { }
        private class Service540 : IService540 { }
        private interface IService541 { }
        private class Service541 : IService541 { }
        private interface IService542 { }
        private class Service542 : IService542 { }
        private interface IService543 { }
        private class Service543 : IService543 { }
        private interface IService544 { }
        private class Service544 : IService544 { }
        private interface IService545 { }
        private class Service545 : IService545 { }
        private interface IService546 { }
        private class Service546 : IService546 { }
        private interface IService547 { }
        private class Service547 : IService547 { }
        private interface IService548 { }
        private class Service548 : IService548 { }
        private interface IService549 { }
        private class Service549 : IService549 { }
        private interface IService550 { }
        private class Service550 : IService550 { }
        private interface IService551 { }
        private class Service551 : IService551 { }
        private interface IService552 { }
        private class Service552 : IService552 { }
        private interface IService553 { }
        private class Service553 : IService553 { }
        private interface IService554 { }
        private class Service554 : IService554 { }
        private interface IService555 { }
        private class Service555 : IService555 { }
        private interface IService556 { }
        private class Service556 : IService556 { }
        private interface IService557 { }
        private class Service557 : IService557 { }
        private interface IService558 { }
        private class Service558 : IService558 { }
        private interface IService559 { }
        private class Service559 : IService559 { }
        private interface IService560 { }
        private class Service560 : IService560 { }
        private interface IService561 { }
        private class Service561 : IService561 { }
        private interface IService562 { }
        private class Service562 : IService562 { }
        private interface IService563 { }
        private class Service563 : IService563 { }
        private interface IService564 { }
        private class Service564 : IService564 { }
        private interface IService565 { }
        private class Service565 : IService565 { }
        private interface IService566 { }
        private class Service566 : IService566 { }
        private interface IService567 { }
        private class Service567 : IService567 { }
        private interface IService568 { }
        private class Service568 : IService568 { }
        private interface IService569 { }
        private class Service569 : IService569 { }
        private interface IService570 { }
        private class Service570 : IService570 { }
        private interface IService571 { }
        private class Service571 : IService571 { }
        private interface IService572 { }
        private class Service572 : IService572 { }
        private interface IService573 { }
        private class Service573 : IService573 { }
        private interface IService574 { }
        private class Service574 : IService574 { }
        private interface IService575 { }
        private class Service575 : IService575 { }
        private interface IService576 { }
        private class Service576 : IService576 { }
        private interface IService577 { }
        private class Service577 : IService577 { }
        private interface IService578 { }
        private class Service578 : IService578 { }
        private interface IService579 { }
        private class Service579 : IService579 { }
        private interface IService580 { }
        private class Service580 : IService580 { }
        private interface IService581 { }
        private class Service581 : IService581 { }
        private interface IService582 { }
        private class Service582 : IService582 { }
        private interface IService583 { }
        private class Service583 : IService583 { }
        private interface IService584 { }
        private class Service584 : IService584 { }
        private interface IService585 { }
        private class Service585 : IService585 { }
        private interface IService586 { }
        private class Service586 : IService586 { }
        private interface IService587 { }
        private class Service587 : IService587 { }
        private interface IService588 { }
        private class Service588 : IService588 { }
        private interface IService589 { }
        private class Service589 : IService589 { }
        private interface IService590 { }
        private class Service590 : IService590 { }
        private interface IService591 { }
        private class Service591 : IService591 { }
        private interface IService592 { }
        private class Service592 : IService592 { }
        private interface IService593 { }
        private class Service593 : IService593 { }
        private interface IService594 { }
        private class Service594 : IService594 { }
        private interface IService595 { }
        private class Service595 : IService595 { }
        private interface IService596 { }
        private class Service596 : IService596 { }
        private interface IService597 { }
        private class Service597 : IService597 { }
        private interface IService598 { }
        private class Service598 : IService598 { }
        private interface IService599 { }
        private class Service599 : IService599 { }
        private interface IService600 { }
        private class Service600 : IService600 { }
        private interface IService601 { }
        private class Service601 : IService601 { }
        private interface IService602 { }
        private class Service602 : IService602 { }
        private interface IService603 { }
        private class Service603 : IService603 { }
        private interface IService604 { }
        private class Service604 : IService604 { }
        private interface IService605 { }
        private class Service605 : IService605 { }
        private interface IService606 { }
        private class Service606 : IService606 { }
        private interface IService607 { }
        private class Service607 : IService607 { }
        private interface IService608 { }
        private class Service608 : IService608 { }
        private interface IService609 { }
        private class Service609 : IService609 { }
        private interface IService610 { }
        private class Service610 : IService610 { }
        private interface IService611 { }
        private class Service611 : IService611 { }
        private interface IService612 { }
        private class Service612 : IService612 { }
        private interface IService613 { }
        private class Service613 : IService613 { }
        private interface IService614 { }
        private class Service614 : IService614 { }
        private interface IService615 { }
        private class Service615 : IService615 { }
        private interface IService616 { }
        private class Service616 : IService616 { }
        private interface IService617 { }
        private class Service617 : IService617 { }
        private interface IService618 { }
        private class Service618 : IService618 { }
        private interface IService619 { }
        private class Service619 : IService619 { }
        private interface IService620 { }
        private class Service620 : IService620 { }
        private interface IService621 { }
        private class Service621 : IService621 { }
        private interface IService622 { }
        private class Service622 : IService622 { }
        private interface IService623 { }
        private class Service623 : IService623 { }
        private interface IService624 { }
        private class Service624 : IService624 { }
        private interface IService625 { }
        private class Service625 : IService625 { }
        private interface IService626 { }
        private class Service626 : IService626 { }
        private interface IService627 { }
        private class Service627 : IService627 { }
        private interface IService628 { }
        private class Service628 : IService628 { }
        private interface IService629 { }
        private class Service629 : IService629 { }
        private interface IService630 { }
        private class Service630 : IService630 { }
        private interface IService631 { }
        private class Service631 : IService631 { }
        private interface IService632 { }
        private class Service632 : IService632 { }
        private interface IService633 { }
        private class Service633 : IService633 { }
        private interface IService634 { }
        private class Service634 : IService634 { }
        private interface IService635 { }
        private class Service635 : IService635 { }
        private interface IService636 { }
        private class Service636 : IService636 { }
        private interface IService637 { }
        private class Service637 : IService637 { }
        private interface IService638 { }
        private class Service638 : IService638 { }
        private interface IService639 { }
        private class Service639 : IService639 { }
        private interface IService640 { }
        private class Service640 : IService640 { }
        private interface IService641 { }
        private class Service641 : IService641 { }
        private interface IService642 { }
        private class Service642 : IService642 { }
        private interface IService643 { }
        private class Service643 : IService643 { }
        private interface IService644 { }
        private class Service644 : IService644 { }
        private interface IService645 { }
        private class Service645 : IService645 { }
        private interface IService646 { }
        private class Service646 : IService646 { }
        private interface IService647 { }
        private class Service647 : IService647 { }
        private interface IService648 { }
        private class Service648 : IService648 { }
        private interface IService649 { }
        private class Service649 : IService649 { }
        private interface IService650 { }
        private class Service650 : IService650 { }
        private interface IService651 { }
        private class Service651 : IService651 { }
        private interface IService652 { }
        private class Service652 : IService652 { }
        private interface IService653 { }
        private class Service653 : IService653 { }
        private interface IService654 { }
        private class Service654 : IService654 { }
        private interface IService655 { }
        private class Service655 : IService655 { }
        private interface IService656 { }
        private class Service656 : IService656 { }
        private interface IService657 { }
        private class Service657 : IService657 { }
        private interface IService658 { }
        private class Service658 : IService658 { }
        private interface IService659 { }
        private class Service659 : IService659 { }
        private interface IService660 { }
        private class Service660 : IService660 { }
        private interface IService661 { }
        private class Service661 : IService661 { }
        private interface IService662 { }
        private class Service662 : IService662 { }
        private interface IService663 { }
        private class Service663 : IService663 { }
        private interface IService664 { }
        private class Service664 : IService664 { }
        private interface IService665 { }
        private class Service665 : IService665 { }
        private interface IService666 { }
        private class Service666 : IService666 { }
        private interface IService667 { }
        private class Service667 : IService667 { }
        private interface IService668 { }
        private class Service668 : IService668 { }
        private interface IService669 { }
        private class Service669 : IService669 { }
        private interface IService670 { }
        private class Service670 : IService670 { }
        private interface IService671 { }
        private class Service671 : IService671 { }
        private interface IService672 { }
        private class Service672 : IService672 { }
        private interface IService673 { }
        private class Service673 : IService673 { }
        private interface IService674 { }
        private class Service674 : IService674 { }
        private interface IService675 { }
        private class Service675 : IService675 { }
        private interface IService676 { }
        private class Service676 : IService676 { }
        private interface IService677 { }
        private class Service677 : IService677 { }
        private interface IService678 { }
        private class Service678 : IService678 { }
        private interface IService679 { }
        private class Service679 : IService679 { }
        private interface IService680 { }
        private class Service680 : IService680 { }
        private interface IService681 { }
        private class Service681 : IService681 { }
        private interface IService682 { }
        private class Service682 : IService682 { }
        private interface IService683 { }
        private class Service683 : IService683 { }
        private interface IService684 { }
        private class Service684 : IService684 { }
        private interface IService685 { }
        private class Service685 : IService685 { }
        private interface IService686 { }
        private class Service686 : IService686 { }
        private interface IService687 { }
        private class Service687 : IService687 { }
        private interface IService688 { }
        private class Service688 : IService688 { }
        private interface IService689 { }
        private class Service689 : IService689 { }
        private interface IService690 { }
        private class Service690 : IService690 { }
        private interface IService691 { }
        private class Service691 : IService691 { }
        private interface IService692 { }
        private class Service692 : IService692 { }
        private interface IService693 { }
        private class Service693 : IService693 { }
        private interface IService694 { }
        private class Service694 : IService694 { }
        private interface IService695 { }
        private class Service695 : IService695 { }
        private interface IService696 { }
        private class Service696 : IService696 { }
        private interface IService697 { }
        private class Service697 : IService697 { }
        private interface IService698 { }
        private class Service698 : IService698 { }
        private interface IService699 { }
        private class Service699 : IService699 { }
        private interface IService700 { }
        private class Service700 : IService700 { }
        private interface IService701 { }
        private class Service701 : IService701 { }
        private interface IService702 { }
        private class Service702 : IService702 { }
        private interface IService703 { }
        private class Service703 : IService703 { }
        private interface IService704 { }
        private class Service704 : IService704 { }
        private interface IService705 { }
        private class Service705 : IService705 { }
        private interface IService706 { }
        private class Service706 : IService706 { }
        private interface IService707 { }
        private class Service707 : IService707 { }
        private interface IService708 { }
        private class Service708 : IService708 { }
        private interface IService709 { }
        private class Service709 : IService709 { }
        private interface IService710 { }
        private class Service710 : IService710 { }
        private interface IService711 { }
        private class Service711 : IService711 { }
        private interface IService712 { }
        private class Service712 : IService712 { }
        private interface IService713 { }
        private class Service713 : IService713 { }
        private interface IService714 { }
        private class Service714 : IService714 { }
        private interface IService715 { }
        private class Service715 : IService715 { }
        private interface IService716 { }
        private class Service716 : IService716 { }
        private interface IService717 { }
        private class Service717 : IService717 { }
        private interface IService718 { }
        private class Service718 : IService718 { }
        private interface IService719 { }
        private class Service719 : IService719 { }
        private interface IService720 { }
        private class Service720 : IService720 { }
        private interface IService721 { }
        private class Service721 : IService721 { }
        private interface IService722 { }
        private class Service722 : IService722 { }
        private interface IService723 { }
        private class Service723 : IService723 { }
        private interface IService724 { }
        private class Service724 : IService724 { }
        private interface IService725 { }
        private class Service725 : IService725 { }
        private interface IService726 { }
        private class Service726 : IService726 { }
        private interface IService727 { }
        private class Service727 : IService727 { }
        private interface IService728 { }
        private class Service728 : IService728 { }
        private interface IService729 { }
        private class Service729 : IService729 { }
        private interface IService730 { }
        private class Service730 : IService730 { }
        private interface IService731 { }
        private class Service731 : IService731 { }
        private interface IService732 { }
        private class Service732 : IService732 { }
        private interface IService733 { }
        private class Service733 : IService733 { }
        private interface IService734 { }
        private class Service734 : IService734 { }
        private interface IService735 { }
        private class Service735 : IService735 { }
        private interface IService736 { }
        private class Service736 : IService736 { }
        private interface IService737 { }
        private class Service737 : IService737 { }
        private interface IService738 { }
        private class Service738 : IService738 { }
        private interface IService739 { }
        private class Service739 : IService739 { }
        private interface IService740 { }
        private class Service740 : IService740 { }
        private interface IService741 { }
        private class Service741 : IService741 { }
        private interface IService742 { }
        private class Service742 : IService742 { }
        private interface IService743 { }
        private class Service743 : IService743 { }
        private interface IService744 { }
        private class Service744 : IService744 { }
        private interface IService745 { }
        private class Service745 : IService745 { }
        private interface IService746 { }
        private class Service746 : IService746 { }
        private interface IService747 { }
        private class Service747 : IService747 { }
        private interface IService748 { }
        private class Service748 : IService748 { }
        private interface IService749 { }
        private class Service749 : IService749 { }
        private interface IService750 { }
        private class Service750 : IService750 { }
        private interface IService751 { }
        private class Service751 : IService751 { }
        private interface IService752 { }
        private class Service752 : IService752 { }
        private interface IService753 { }
        private class Service753 : IService753 { }
        private interface IService754 { }
        private class Service754 : IService754 { }
        private interface IService755 { }
        private class Service755 : IService755 { }
        private interface IService756 { }
        private class Service756 : IService756 { }
        private interface IService757 { }
        private class Service757 : IService757 { }
        private interface IService758 { }
        private class Service758 : IService758 { }
        private interface IService759 { }
        private class Service759 : IService759 { }
        private interface IService760 { }
        private class Service760 : IService760 { }
        private interface IService761 { }
        private class Service761 : IService761 { }
        private interface IService762 { }
        private class Service762 : IService762 { }
        private interface IService763 { }
        private class Service763 : IService763 { }
        private interface IService764 { }
        private class Service764 : IService764 { }
        private interface IService765 { }
        private class Service765 : IService765 { }
        private interface IService766 { }
        private class Service766 : IService766 { }
        private interface IService767 { }
        private class Service767 : IService767 { }
        private interface IService768 { }
        private class Service768 : IService768 { }
        private interface IService769 { }
        private class Service769 : IService769 { }
        private interface IService770 { }
        private class Service770 : IService770 { }
        private interface IService771 { }
        private class Service771 : IService771 { }
        private interface IService772 { }
        private class Service772 : IService772 { }
        private interface IService773 { }
        private class Service773 : IService773 { }
        private interface IService774 { }
        private class Service774 : IService774 { }
        private interface IService775 { }
        private class Service775 : IService775 { }
        private interface IService776 { }
        private class Service776 : IService776 { }
        private interface IService777 { }
        private class Service777 : IService777 { }
        private interface IService778 { }
        private class Service778 : IService778 { }
        private interface IService779 { }
        private class Service779 : IService779 { }
        private interface IService780 { }
        private class Service780 : IService780 { }
        private interface IService781 { }
        private class Service781 : IService781 { }
        private interface IService782 { }
        private class Service782 : IService782 { }
        private interface IService783 { }
        private class Service783 : IService783 { }
        private interface IService784 { }
        private class Service784 : IService784 { }
        private interface IService785 { }
        private class Service785 : IService785 { }
        private interface IService786 { }
        private class Service786 : IService786 { }
        private interface IService787 { }
        private class Service787 : IService787 { }
        private interface IService788 { }
        private class Service788 : IService788 { }
        private interface IService789 { }
        private class Service789 : IService789 { }
        private interface IService790 { }
        private class Service790 : IService790 { }
        private interface IService791 { }
        private class Service791 : IService791 { }
        private interface IService792 { }
        private class Service792 : IService792 { }
        private interface IService793 { }
        private class Service793 : IService793 { }
        private interface IService794 { }
        private class Service794 : IService794 { }
        private interface IService795 { }
        private class Service795 : IService795 { }
        private interface IService796 { }
        private class Service796 : IService796 { }
        private interface IService797 { }
        private class Service797 : IService797 { }
        private interface IService798 { }
        private class Service798 : IService798 { }
        private interface IService799 { }
        private class Service799 : IService799 { }
        private interface IService800 { }
        private class Service800 : IService800 { }
        private interface IService801 { }
        private class Service801 : IService801 { }
        private interface IService802 { }
        private class Service802 : IService802 { }
        private interface IService803 { }
        private class Service803 : IService803 { }
        private interface IService804 { }
        private class Service804 : IService804 { }
        private interface IService805 { }
        private class Service805 : IService805 { }
        private interface IService806 { }
        private class Service806 : IService806 { }
        private interface IService807 { }
        private class Service807 : IService807 { }
        private interface IService808 { }
        private class Service808 : IService808 { }
        private interface IService809 { }
        private class Service809 : IService809 { }
        private interface IService810 { }
        private class Service810 : IService810 { }
        private interface IService811 { }
        private class Service811 : IService811 { }
        private interface IService812 { }
        private class Service812 : IService812 { }
        private interface IService813 { }
        private class Service813 : IService813 { }
        private interface IService814 { }
        private class Service814 : IService814 { }
        private interface IService815 { }
        private class Service815 : IService815 { }
        private interface IService816 { }
        private class Service816 : IService816 { }
        private interface IService817 { }
        private class Service817 : IService817 { }
        private interface IService818 { }
        private class Service818 : IService818 { }
        private interface IService819 { }
        private class Service819 : IService819 { }
        private interface IService820 { }
        private class Service820 : IService820 { }
        private interface IService821 { }
        private class Service821 : IService821 { }
        private interface IService822 { }
        private class Service822 : IService822 { }
        private interface IService823 { }
        private class Service823 : IService823 { }
        private interface IService824 { }
        private class Service824 : IService824 { }
        private interface IService825 { }
        private class Service825 : IService825 { }
        private interface IService826 { }
        private class Service826 : IService826 { }
        private interface IService827 { }
        private class Service827 : IService827 { }
        private interface IService828 { }
        private class Service828 : IService828 { }
        private interface IService829 { }
        private class Service829 : IService829 { }
        private interface IService830 { }
        private class Service830 : IService830 { }
        private interface IService831 { }
        private class Service831 : IService831 { }
        private interface IService832 { }
        private class Service832 : IService832 { }
        private interface IService833 { }
        private class Service833 : IService833 { }
        private interface IService834 { }
        private class Service834 : IService834 { }
        private interface IService835 { }
        private class Service835 : IService835 { }
        private interface IService836 { }
        private class Service836 : IService836 { }
        private interface IService837 { }
        private class Service837 : IService837 { }
        private interface IService838 { }
        private class Service838 : IService838 { }
        private interface IService839 { }
        private class Service839 : IService839 { }
        private interface IService840 { }
        private class Service840 : IService840 { }
        private interface IService841 { }
        private class Service841 : IService841 { }
        private interface IService842 { }
        private class Service842 : IService842 { }
        private interface IService843 { }
        private class Service843 : IService843 { }
        private interface IService844 { }
        private class Service844 : IService844 { }
        private interface IService845 { }
        private class Service845 : IService845 { }
        private interface IService846 { }
        private class Service846 : IService846 { }
        private interface IService847 { }
        private class Service847 : IService847 { }
        private interface IService848 { }
        private class Service848 : IService848 { }
        private interface IService849 { }
        private class Service849 : IService849 { }
        private interface IService850 { }
        private class Service850 : IService850 { }
        private interface IService851 { }
        private class Service851 : IService851 { }
        private interface IService852 { }
        private class Service852 : IService852 { }
        private interface IService853 { }
        private class Service853 : IService853 { }
        private interface IService854 { }
        private class Service854 : IService854 { }
        private interface IService855 { }
        private class Service855 : IService855 { }
        private interface IService856 { }
        private class Service856 : IService856 { }
        private interface IService857 { }
        private class Service857 : IService857 { }
        private interface IService858 { }
        private class Service858 : IService858 { }
        private interface IService859 { }
        private class Service859 : IService859 { }
        private interface IService860 { }
        private class Service860 : IService860 { }
        private interface IService861 { }
        private class Service861 : IService861 { }
        private interface IService862 { }
        private class Service862 : IService862 { }
        private interface IService863 { }
        private class Service863 : IService863 { }
        private interface IService864 { }
        private class Service864 : IService864 { }
        private interface IService865 { }
        private class Service865 : IService865 { }
        private interface IService866 { }
        private class Service866 : IService866 { }
        private interface IService867 { }
        private class Service867 : IService867 { }
        private interface IService868 { }
        private class Service868 : IService868 { }
        private interface IService869 { }
        private class Service869 : IService869 { }
        private interface IService870 { }
        private class Service870 : IService870 { }
        private interface IService871 { }
        private class Service871 : IService871 { }
        private interface IService872 { }
        private class Service872 : IService872 { }
        private interface IService873 { }
        private class Service873 : IService873 { }
        private interface IService874 { }
        private class Service874 : IService874 { }
        private interface IService875 { }
        private class Service875 : IService875 { }
        private interface IService876 { }
        private class Service876 : IService876 { }
        private interface IService877 { }
        private class Service877 : IService877 { }
        private interface IService878 { }
        private class Service878 : IService878 { }
        private interface IService879 { }
        private class Service879 : IService879 { }
        private interface IService880 { }
        private class Service880 : IService880 { }
        private interface IService881 { }
        private class Service881 : IService881 { }
        private interface IService882 { }
        private class Service882 : IService882 { }
        private interface IService883 { }
        private class Service883 : IService883 { }
        private interface IService884 { }
        private class Service884 : IService884 { }
        private interface IService885 { }
        private class Service885 : IService885 { }
        private interface IService886 { }
        private class Service886 : IService886 { }
        private interface IService887 { }
        private class Service887 : IService887 { }
        private interface IService888 { }
        private class Service888 : IService888 { }
        private interface IService889 { }
        private class Service889 : IService889 { }
        private interface IService890 { }
        private class Service890 : IService890 { }
        private interface IService891 { }
        private class Service891 : IService891 { }
        private interface IService892 { }
        private class Service892 : IService892 { }
        private interface IService893 { }
        private class Service893 : IService893 { }
        private interface IService894 { }
        private class Service894 : IService894 { }
        private interface IService895 { }
        private class Service895 : IService895 { }
        private interface IService896 { }
        private class Service896 : IService896 { }
        private interface IService897 { }
        private class Service897 : IService897 { }
        private interface IService898 { }
        private class Service898 : IService898 { }
        private interface IService899 { }
        private class Service899 : IService899 { }
        private interface IService900 { }
        private class Service900 : IService900 { }
        private interface IService901 { }
        private class Service901 : IService901 { }
        private interface IService902 { }
        private class Service902 : IService902 { }
        private interface IService903 { }
        private class Service903 : IService903 { }
        private interface IService904 { }
        private class Service904 : IService904 { }
        private interface IService905 { }
        private class Service905 : IService905 { }
        private interface IService906 { }
        private class Service906 : IService906 { }
        private interface IService907 { }
        private class Service907 : IService907 { }
        private interface IService908 { }
        private class Service908 : IService908 { }
        private interface IService909 { }
        private class Service909 : IService909 { }
        private interface IService910 { }
        private class Service910 : IService910 { }
        private interface IService911 { }
        private class Service911 : IService911 { }
        private interface IService912 { }
        private class Service912 : IService912 { }
        private interface IService913 { }
        private class Service913 : IService913 { }
        private interface IService914 { }
        private class Service914 : IService914 { }
        private interface IService915 { }
        private class Service915 : IService915 { }
        private interface IService916 { }
        private class Service916 : IService916 { }
        private interface IService917 { }
        private class Service917 : IService917 { }
        private interface IService918 { }
        private class Service918 : IService918 { }
        private interface IService919 { }
        private class Service919 : IService919 { }
        private interface IService920 { }
        private class Service920 : IService920 { }
        private interface IService921 { }
        private class Service921 : IService921 { }
        private interface IService922 { }
        private class Service922 : IService922 { }
        private interface IService923 { }
        private class Service923 : IService923 { }
        private interface IService924 { }
        private class Service924 : IService924 { }
        private interface IService925 { }
        private class Service925 : IService925 { }
        private interface IService926 { }
        private class Service926 : IService926 { }
        private interface IService927 { }
        private class Service927 : IService927 { }
        private interface IService928 { }
        private class Service928 : IService928 { }
        private interface IService929 { }
        private class Service929 : IService929 { }
        private interface IService930 { }
        private class Service930 : IService930 { }
        private interface IService931 { }
        private class Service931 : IService931 { }
        private interface IService932 { }
        private class Service932 : IService932 { }
        private interface IService933 { }
        private class Service933 : IService933 { }
        private interface IService934 { }
        private class Service934 : IService934 { }
        private interface IService935 { }
        private class Service935 : IService935 { }
        private interface IService936 { }
        private class Service936 : IService936 { }
        private interface IService937 { }
        private class Service937 : IService937 { }
        private interface IService938 { }
        private class Service938 : IService938 { }
        private interface IService939 { }
        private class Service939 : IService939 { }
        private interface IService940 { }
        private class Service940 : IService940 { }
        private interface IService941 { }
        private class Service941 : IService941 { }
        private interface IService942 { }
        private class Service942 : IService942 { }
        private interface IService943 { }
        private class Service943 : IService943 { }
        private interface IService944 { }
        private class Service944 : IService944 { }
        private interface IService945 { }
        private class Service945 : IService945 { }
        private interface IService946 { }
        private class Service946 : IService946 { }
        private interface IService947 { }
        private class Service947 : IService947 { }
        private interface IService948 { }
        private class Service948 : IService948 { }
        private interface IService949 { }
        private class Service949 : IService949 { }
        private interface IService950 { }
        private class Service950 : IService950 { }
        private interface IService951 { }
        private class Service951 : IService951 { }
        private interface IService952 { }
        private class Service952 : IService952 { }
        private interface IService953 { }
        private class Service953 : IService953 { }
        private interface IService954 { }
        private class Service954 : IService954 { }
        private interface IService955 { }
        private class Service955 : IService955 { }
        private interface IService956 { }
        private class Service956 : IService956 { }
        private interface IService957 { }
        private class Service957 : IService957 { }
        private interface IService958 { }
        private class Service958 : IService958 { }
        private interface IService959 { }
        private class Service959 : IService959 { }
        private interface IService960 { }
        private class Service960 : IService960 { }
        private interface IService961 { }
        private class Service961 : IService961 { }
        private interface IService962 { }
        private class Service962 : IService962 { }
        private interface IService963 { }
        private class Service963 : IService963 { }
        private interface IService964 { }
        private class Service964 : IService964 { }
        private interface IService965 { }
        private class Service965 : IService965 { }
        private interface IService966 { }
        private class Service966 : IService966 { }
        private interface IService967 { }
        private class Service967 : IService967 { }
        private interface IService968 { }
        private class Service968 : IService968 { }
        private interface IService969 { }
        private class Service969 : IService969 { }
        private interface IService970 { }
        private class Service970 : IService970 { }
        private interface IService971 { }
        private class Service971 : IService971 { }
        private interface IService972 { }
        private class Service972 : IService972 { }
        private interface IService973 { }
        private class Service973 : IService973 { }
        private interface IService974 { }
        private class Service974 : IService974 { }
        private interface IService975 { }
        private class Service975 : IService975 { }
        private interface IService976 { }
        private class Service976 : IService976 { }
        private interface IService977 { }
        private class Service977 : IService977 { }
        private interface IService978 { }
        private class Service978 : IService978 { }
        private interface IService979 { }
        private class Service979 : IService979 { }
        private interface IService980 { }
        private class Service980 : IService980 { }
        private interface IService981 { }
        private class Service981 : IService981 { }
        private interface IService982 { }
        private class Service982 : IService982 { }
        private interface IService983 { }
        private class Service983 : IService983 { }
        private interface IService984 { }
        private class Service984 : IService984 { }
        private interface IService985 { }
        private class Service985 : IService985 { }
        private interface IService986 { }
        private class Service986 : IService986 { }
        private interface IService987 { }
        private class Service987 : IService987 { }
        private interface IService988 { }
        private class Service988 : IService988 { }
        private interface IService989 { }
        private class Service989 : IService989 { }
        private interface IService990 { }
        private class Service990 : IService990 { }
        private interface IService991 { }
        private class Service991 : IService991 { }
        private interface IService992 { }
        private class Service992 : IService992 { }
        private interface IService993 { }
        private class Service993 : IService993 { }
        private interface IService994 { }
        private class Service994 : IService994 { }
        private interface IService995 { }
        private class Service995 : IService995 { }
        private interface IService996 { }
        private class Service996 : IService996 { }
        private interface IService997 { }
        private class Service997 : IService997 { }
        private interface IService998 { }
        private class Service998 : IService998 { }
        private interface IService999 { }
        private class Service999 : IService999 { }
    }
}