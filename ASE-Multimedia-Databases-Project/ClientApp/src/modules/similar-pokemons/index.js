import { Form, Button } from 'react-bootstrap'
import { useForm } from 'react-hook-form'
import { useQuery } from 'react-query'
import { toast } from 'react-toastify'
import { PokemonCard } from '../pokemons/PokemonCard'

export const SimilarPokemonsPage = () => {
    const { handleSubmit, register, watch } = useForm({
        defaultValues: {
            images: []
        }
    })

    const { data: pokemons = [], refetch } = useQuery({
        queryKey: ['similar-pokemons'],
        queryFn: async () => {
            const form = watch()
            const images = [...form.images]

            if (!images.length) throw new Error('You need to upload an image')

            const formData = new FormData();

            images.forEach(image => formData.append('Images', image));

            const res = await fetch('/api/pokemon/similar', {
                method: 'POST',
                body: formData
            })
            const data = await res.json()
            return data
        },
        onError(error) {
            toast.error(error.message)
        },
        enabled: false
    })

    const onSubmit = handleSubmit(() => refetch())

    return <>
        <h1>Find similar pokemons</h1>

        <Form onSubmit={onSubmit} className='mt-4'>
            <Form.Group controlId='image'>
                <Form.Label>Upload image</Form.Label>
                <Form.Control
                    type='file'
                    accept='image/png, image/jpg, image/jpeg'
                    {...register('images', { required: true })}
                />
            </Form.Group>

            <Button type='submit' variant='primary' className='mt-4'>Find</Button>
        </Form>

        {Boolean(pokemons.length) &&
            <ol className="d-grid p-0 row-gap-4 column-gap-4 mt-4" style={{ gridTemplateColumns: 'repeat(auto-fill, 15rem)' }}>
                {pokemons.map(pokemon =>
                    <li key={pokemon.id} className="d-block h-100">
                        <PokemonCard {...pokemon} />
                    </li>
                )}
            </ol>
        }
    </>
}