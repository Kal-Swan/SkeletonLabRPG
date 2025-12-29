import * as signalR from "@microsoft/signalr";
import * as v from '$env/dynamic/public';
import { getAccessToken } from "@lib/stores/auth";

export const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`${v.env.PUBLIC_API_URL}/buildHub`,{
        accessTokenFactory: async () => {
            console.log('from buildhub accessTokenFactory');
            console.log(v.env.PUBLIC_API_URL);
            const accessToken = await getAccessToken();
            console.log('access token from buildhub');
            console.log(accessToken);
            return accessToken;

        },
    })
    .withAutomaticReconnect()
    .build();