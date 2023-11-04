import { useQuery } from "react-query"
import { Card } from "react-bootstrap"
import { PokemonImage } from "./PokemonImage"
import { PokemonCard } from "./PokemonCard"

export const PokemonsPage = () => {
    const { data: pokemons, isLoading } = useQuery({
        queryKey: 'pokemons',
        queryFn: async () => {
            const res = await fetch('/api/pokemon', {
                headers: {
                    'Content-Type': 'application/json',
                }
            })
            const data = await res.json()
            return data
        }
    })

    if(isLoading) return <div>Loading...</div>

    return <>
        <h1 className="mb-4">Pokemons</h1>

        <ol className="d-grid p-0 row-gap-4 column-gap-4" style={{ gridTemplateColumns: 'repeat(auto-fill, 15rem)' }}>
            {pokemons.map(pokemon =>
                <li key={pokemon.id} className="d-block h-100">
                    <PokemonCard {...pokemon} />
                </li>
            )}
        </ol>
    </>
}