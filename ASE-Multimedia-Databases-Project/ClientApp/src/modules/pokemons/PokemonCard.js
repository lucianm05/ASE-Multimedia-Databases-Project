import { useCallback, useState } from "react"
import { Card } from "react-bootstrap"
import { PokemonImage } from "./PokemonImage"
import { ChevronLeft, ChevronRight, Speaker } from 'react-bootstrap-icons'
import { PokemonSound } from "./PokemonSound"

export const PokemonCard = ({ name, generation, images, sound }) => {
    const [currentImage, setCurrentImage] = useState(0)

    const onPrevImage = useCallback(() => {
        setCurrentImage(prev => {
            const next = prev - 1
            if (next >= 0) return next
            return 0
        })
    }, [])

    const onNextImage = useCallback(() => {
        setCurrentImage(prev => {
            const next = prev + 1
            if (next < images.length) return next
            return images.length - 1
        })
    }, [images])

    const hasPrevImage = currentImage > 0
    const hasNextImage = currentImage < images.length - 1

    return (
        <Card style={{ maxWidth: '15rem', height: '100%' }}>
            <Card.Body className="d-flex flex-column justify-content-between">
                <div className="position-relative overflow-hidden">
                    <PokemonSound id={sound} className="position-absolute end-0" />

                    {hasPrevImage &&
                        <button type="button" onClick={onPrevImage} className="border-0 position-absolute top-50 translate-middle-y bg-transparent">
                            <ChevronLeft />
                        </button>
                    }

                    <PokemonImage id={images[currentImage]} />

                    {hasNextImage &&
                        <button type="button" onClick={onNextImage} className="border-0 position-absolute top-50 translate-middle-y end-0 bg-transparent">
                            <ChevronRight />
                        </button>
                    }
                </div>
                <div className="mt-2">
                    <Card.Title>
                        {name}
                    </Card.Title>
                    <Card.Text>
                        (Gen. {generation})
                    </Card.Text>
                </div>
            </Card.Body>
        </Card>
    )
}