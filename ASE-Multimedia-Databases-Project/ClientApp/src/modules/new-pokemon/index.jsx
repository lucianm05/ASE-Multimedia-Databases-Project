import { useForm } from 'react-hook-form'
import { Form, Button } from 'react-bootstrap'
import { useMutation } from 'react-query'
import { toast } from 'react-toastify'

export const NewPokemonPage = () => {
    const { register, handleSubmit, reset } = useForm({
        defaultValues: {
            name: '',
            generation: 1,
            images: [],
            sound: undefined
        },
    })

    const { mutate } = useMutation({
        mutationFn: async (data) => {
            const formData = new FormData()

            const images = [...data.images]
            const sounds = [...data.sound]

            formData.append('Name', data.name)
            formData.append('Generation', data.generation)
            images.forEach(image => formData.append('Images', image))
            sounds.forEach(sound => formData.append('Sound', sound))

            const res = await fetch('/api/pokemon', {
                method: 'POST',
                body: formData,
            })
            const { id } = await res.json()

            if (!id) throw new Error('An error occured')

            return id
        },
        onSuccess(data) {
            if (!data) return
            reset({name: '', generation: 1, images: [], sound: null})
            toast.success('Pokemon created')
        },
        onError(error) {
            toast.error(error.message)
        }
    })

    return <>
        <main>
            <h1 className='mb-4'>Add new pokemon</h1>

            <Form onSubmit={handleSubmit(mutate)}>
                <Form.Group controlId='name' className='mb-4'>
                    <Form.Label>
                        Name
                    </Form.Label>
                    <Form.Control type='text' placeholder='Name' {...register('name', { required: true })} />
                </Form.Group>

                <Form.Group controlId='generation' className='mb-4'>
                    <Form.Label>
                        Generation
                    </Form.Label>
                    <Form.Control type='number' placeholder='Generation (1-9)' {...register('generation', { min: 1, max: 9, required: true })} />
                </Form.Group>

                <Form.Group controlId='images' className='mb-4'>
                    <Form.Label>
                        Images
                    </Form.Label>
                    <Form.Control
                        type='file'
                        accept='image/png, image/jpg, image/jpeg'
                        multiple
                        {...register('images', { required: true })}
                    />
                </Form.Group>

                <Form.Group controlId='sound' className='mb-4'>
                    <Form.Label>
                        Sound
                    </Form.Label>
                    <Form.Control
                        type='file'
                        accept='audio/mp3'
                        {...register('sound', { required: true })}
                    />
                </Form.Group>

                <Button type='submit' variant='primary'>Create Pokemon</Button>
            </Form>
        </main>
    </>
}