import { useRef } from "react"
import { VolumeUp } from 'react-bootstrap-icons'

export const PokemonSound = ({ id, className }) => {
    const audioRef = useRef()

    const onClick = () => {
        if (!audioRef.current) return

        audioRef.current.play()
    }

    return (
        <div className={className}>
            <audio ref={audioRef} src={`/api/pokemon/sound/${id}`} hidden />

            <button type="button" className="border-0 bg-transparent" aria-label="Play pokemon sound" onClick={onClick}>
                <VolumeUp />
            </button>
        </div>
    )
}