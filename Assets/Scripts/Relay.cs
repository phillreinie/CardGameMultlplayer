using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Relay : MonoBehaviour
{
    public string code;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => 
        {
            Debug.Log("singned in" + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
   
           Allocation alloc = await RelayService.Instance.CreateAllocationAsync(3);
           code = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
           RelayServerData relayServerData = new RelayServerData(alloc, "dtls");
           NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

           NetworkManager.Singleton.StartHost();

            Debug.Log("Created Relay " + code);


        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinRelay()
    {
        try
        {
           JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(code);
           RelayServerData relayServerData = new RelayServerData(joinAlloc, "dtls");
           NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

           NetworkManager.Singleton.StartClient();

            Debug.Log("Joined Relay " + code);

        } catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
        
    }



}