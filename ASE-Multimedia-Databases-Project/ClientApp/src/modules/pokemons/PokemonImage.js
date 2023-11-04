import { Card } from 'react-bootstrap'

export const PokemonImage = ({ id }) => {
    return <Card.Img src={`/api/pokemon/image/${id}`} style={{height: '12.5rem', width: 'auto', display: 'block', margin:'0 auto'} } />
}