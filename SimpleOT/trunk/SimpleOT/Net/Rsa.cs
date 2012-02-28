using System;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace SimpleOT.Commons.Net
{
	public static class Rsa
	{
        private static readonly RsaEngine EncryptRsaEngine;
		private static readonly RsaEngine DecryptRsaEngine;
		
		//public
		private const String OtE = "65537";
		private const String OtM = "109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413";
		
		//private
		private const String OtP = "14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113";
		private const String OtQ = "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101";
		private const String OtD = "46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073";
		private const String OtDp = "11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937";
		private const String OtDq = "4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973";
		private const String OtInverseq = "5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418";
		
        static Rsa()
        {
            try
            {
                EncryptRsaEngine = new RsaEngine();
                EncryptRsaEngine.Init(true, new RsaKeyParameters(false, new BigInteger(OtM), new BigInteger(OtE)));
				
				DecryptRsaEngine = new RsaEngine();
				DecryptRsaEngine.Init(false, new RsaPrivateCrtKeyParameters(new BigInteger(OtM), new BigInteger(OtE),
					new BigInteger(OtD), new BigInteger(OtP), new BigInteger(OtQ), new BigInteger(OtDp), new BigInteger(OtDq), new BigInteger(OtInverseq)));
				
            }
            catch (Exception e)
            {
                throw new Exception("Error initializing rsa engine.", e);
            }
        }

        public static void Encrypt(Message message, int offset, int length)
        {	
            var encryptedBytes = EncryptRsaEngine.ProcessBlock(message.Buffer, offset, length);
			Array.Copy(encryptedBytes, 0, message.Buffer, offset, length);
        }

		public static void Decrypt(Message message, int offset, int length) 
		{
		    var decryptedBytes = DecryptRsaEngine.ProcessBlock(message.Buffer, offset, length);
			Array.Copy(decryptedBytes, 0, message.Buffer, offset, decryptedBytes.Length);
		}
	}
}

