namespace Microsoft.Hpc
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Hpc.Rest;

    public static class BatchRegistryExtension
    {
        
        public static async Task<string> GetBatchAADInstanceAsync(this IHpcContext context, string batchNode = null)
            =>
                await
                    context.GetBatchAux(
                        () => context.Registry.GetValueAsync<string>(HpcConstants.HpcFullKeyName, HpcConstants.BatchAADInstance, context.CancellationToken),
                        c => c.GetAadInstanceAsync(context.CancellationToken),
                        batchNode).ConfigureAwait(false);

        public static async Task<string> GetBatchAADTenantIdAsync(this IHpcContext context, string batchNode = null)
            =>
                await
                    context.GetBatchAux(
                        () => context.Registry.GetValueAsync<string>(HpcConstants.HpcFullKeyName, HpcConstants.BatchAADTenantId, context.CancellationToken),
                        c => c.GetAadTenantIdAsync(context.CancellationToken),
                        batchNode).ConfigureAwait(false);

        public static async Task<string> GetBatchAADClientAppIdAsync(this IHpcContext context, string batchNode = null)
            =>
                await
                    context.GetBatchAux(
                        () => context.Registry.GetValueAsync<string>(HpcConstants.HpcFullKeyName, HpcConstants.BatchAADClientAppId, context.CancellationToken),
                        c => c.GetAadClientAppIdAsync(context.CancellationToken),
                        batchNode).ConfigureAwait(false);

        public static async Task<string> GetBatchAADClientAppKeyAsync(this IHpcContext context, string batchNode = null)
            =>
                await
                    context.GetBatchAux(
                        () => context.Registry.GetValueAsync<string>(HpcConstants.HpcFullKeyName, HpcConstants.BatchAADClientAppKey, context.CancellationToken),
                        c => c.GetAadClientAppKeyAsync(context.CancellationToken),
                        batchNode).ConfigureAwait(false);

        private static async Task<T> GetBatchAux<T>(this IHpcContext context, Func<Task<T>> funcRegistry, Func<HpcBatchRestClient, Task<T>> funcRest, string batchNode)
        {
            if (context.FabricContext.IsHpcHeadNodeService())
            {
                return await funcRegistry().ConfigureAwait(false);
            }
            else
            {
                HpcBatchRestClient client;

                if (string.IsNullOrEmpty(batchNode))
                {
                    client = new HpcBatchRestClient(context);
                }
                else
                {
                    client = new HpcBatchRestClient(batchNode);
                }

                return await funcRest(client).ConfigureAwait(false);
            }
        }
    }
}