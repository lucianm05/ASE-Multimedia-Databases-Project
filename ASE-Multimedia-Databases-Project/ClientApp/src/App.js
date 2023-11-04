import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { PokemonsPage } from './modules/pokemons'
import { NewPokemonPage } from './modules/new-pokemon';
import { SimilarPokemonsPage } from './modules/similar-pokemons'
import './custom.css'
import { QueryClient, QueryClientProvider } from 'react-query';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const queryClient = new QueryClient()

export default class App extends Component {
  static displayName = App.name;

  render () {
      return (
          <QueryClientProvider client={queryClient}>
              <ToastContainer />
                
              <Layout>
                  <Route exact path='/' component={PokemonsPage} />
                  <Route exact path='/new-pokemon' component={NewPokemonPage} />
                  <Route exact path='/similar-pokemons' component={SimilarPokemonsPage} />
              </Layout>
          </QueryClientProvider>   
    );
  }
}
