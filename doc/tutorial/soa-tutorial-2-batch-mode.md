# Microsoft Telepathy SOA Tutorial II – Batch mode

In the [last tutorial](soa-tutorial-1-write-your-first-soa-service-and-client.md), we implemented a simple SOA client and service. However in most cases, the HPC algorithm is not as simple as adding two numbers. Some services are more complex and run for hours. For such services, the end user normally submits all the requests, then retrieves the responses in a couple of hours. This represents a challenge for the developers. For example, can we close the client application after sending all the requests, and then start another client to get the responses the next day? This blog post is to guide you to design the application to handle such scenarios.

The key requirement of the batch mode is to make sure the requests won’t be lost after being successfully sent, and the result generated by the service won’t be lost either.

Let’s create a new service to explain how to create such reliable computations. We create a service which can do prime factorization. Then, we will write a client application to submit 100 big numbers to the service, and then start another client application to retrieve the results.

## Implement the service

Download the [accompanying code sample](../../samples/Batch%20Mode) to build the service.

It’s as simple as the first example.

Here is the service contract:

```csharp
[ServiceContract]
public interface IPrimeFactorization
{
    [OperationContract]
    List<int> Factorize(int n);
}
```

Here is the service implementation:

```csharp
public List<int> Factorize(int n)
{
    List<int> factors = new List<int>();
    for (int i = 2; n > 1; )
    {
        if (n % i == 0)
        {
            factors.Add(i);
            n /= i;
        }
        else
        {
            i++;
        }
    }
    return factors;
}
```

## Implement the client to send requests

Microsoft Telepathy provides a feature called DurableSession. It makes sure all the requests and the responses are persisted.

Let’s write a client application to send the requests.

1. First we need to prepare the session info. This is the same as in the previous tutorial.

   ```csharp
   SessionStartInfo info = new SessionStartInfo("head.contoso.com", "PrimeFactorizationService"); 
   ```

2. Now we need to create a DurableSession object, like the following code.

   ```csharp
   //Create a durable session
   DurableSession session = DurableSession.CreateSession(info);
   Console.WriteLine("Session {0} has been created", session.Id);
   ```

3. Create a BrokerClient object to send requests.

   ```csharp
   //Send batch request
   Random random = new Random();
   const int numRequests = 100;

   using (BrokerClient<IPrimeFactorization> client = new BrokerClient<IPrimeFactorization>(session))
   {
       Console.WriteLine("Sending {0} requests...", numRequests);
       for (int i = 0; i < numRequests; i++)
       {
           int number = random.Next(1, Int32.MaxValue);

           FactorizeRequest request = new FactorizeRequest(number);

           //The second param is used to identify each request.
           //It can be retrieved from the response. 
           client.SendRequest<FactorizeRequest>(request, number);
       }

       client.EndRequests();
       Console.WriteLine("All the {0} requests have been sent", numRequests);
   }
   ```

   Here we call `EndRequests()` to indicate that we have submitted all the requests. After calling `EndRequests()` you cannot use this client object to send more requests.

   Now we can finish this application. Because we used a durable session, it’s fine to close the client. All the requests are still on service side and will be calculated by the compute nodes in the cluster.

## Implement the client to retrieve responses

After a couple of hours, the user can come back and retrieve the results. (In our case you don’t have to wait that long.)

1. Prepare the session info. Note that you need the session ID of the previous session to help the client to attach the session.

   ```csharp
   //Input sessionId here
   string sessionId;
   Console.Write("Input the session id : ");
   sessionId = Console.ReadLine();

   //Change the headnode name here
   SessionAttachInfo info = new SessionAttachInfo("head.contoso.com", sessionId);
   ```

2. Instead of creating a new session, attach to the existing session.

   ```csharp
   //Attach to session
   DurableSession session = DurableSession.AttachSession(info);
   ```

3. Get responses from the session.

   ```csharp
   //Get responses
   using (BrokerClient<IPrimeFactorization> client = new BrokerClient<IPrimeFactorization>(session))
   {
       foreach (BrokerResponse<FactorizeResponse> response in client.GetResponses<FactorizeResponse>())
       {
           int number = response.GetUserData<int>();
           int[] factors = response.Result.FactorizeResult;
           Console.WriteLine("{0} = {1}", number, string.Join<int>(" * ", factors));
       }
   }
   ```

4. Close the session.

## Tips

- Because of persistence of requests and responses, the performance of the durable session is slightly slower than that of the interactive session.

- `session.Close(true)` purges all the persisted data related to this session. It should only be called when the session and its data are not needed any more. If the session will be used going forward, you should call `session.close(false)`.

- If `EndRequests()` is not called, the service will keep waiting for the upcoming request. After a period of time (by default, 5 minutes), it will remove the client if it does not receive more requests. All the requests sent by this client will be removed.