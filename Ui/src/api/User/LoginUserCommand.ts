import { LoginRequest } from '../apiclients/ZincApiClient'
import { createClient } from '../apiClient'

export async function LoginUser(username: string, password: string) {
    const request = new LoginRequest({
        username: username,
        passwordRaw: password
    })

    const client = createClient()
    return await client.login(request)
}
