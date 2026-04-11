import { RegisterNewUserRequest } from '../apiclients/ZincApiClient'
import { createClient } from '../apiClient'

export async function RegisterUser(firstName: string, lastName: string, username: string, password: string) {
    const request = new RegisterNewUserRequest({
        firstName: firstName,
        lastName: lastName,
        username: username,
        passwordRaw: password
    })

    const client = createClient()
    return await client.register(request)
}
