using Grpc.Core;

namespace MtdKey.Cipher.gRPC.Server.Services
{
    public class CipherService : Cipher.CipherBase
    {
        private readonly ILogger<CipherService> _logger;
        private readonly IAesManager aesManager;

        public CipherService(ILogger<CipherService> logger, IAesManager aesManager)
        {
            _logger = logger;
            this.aesManager = aesManager;
        }

        public override Task<TokenData> Encrypt(TargetData target, ServerCallContext context)
        {
            var message = aesManager.EncryptModel(new TokenModel
            {
                Data = target.Message
            });

            return Task.FromResult(new TokenData
            {
                Message = message
            });
        }

        public override Task<TargetData> Decrypt(TokenData token, ServerCallContext context)
        {            
            var model = aesManager.DecryptModel(token.Message);
            
            return Task.FromResult(new TargetData
            {
                Message = model.Data
            });
        }
    }
}
