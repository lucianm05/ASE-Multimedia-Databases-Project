# ASE-Multimedia-Databases-Project

## The aim of the project was to work with multimedia files, stored in the database.

Technologies used: Oracle SQL, ASP.NET Web Api, React.js.

### Database structure:

```
create table p_pokemons(
    id number,
    name varchar2(25),
    generation number,
    evolution_id number,
    check (generation between 1 and 9),
    primary key (id),
    foreign key (evolution_id) references p_pokemons
);
```

```
create table p_pokemons_images(
    id number,
    pokemon_id number,
    image ordsys.ordimage,
    image_signature ordsys.ordimagesignature,
    primary key (id),
    foreign key (pokemon_id) references p_pokemons
);
```

```
create table p_pokemons_sounds (
    id number,
    pokemon_id number,
    sound ordaudio,
    primary key (id),
    foreign key (pokemon_id) references p_pokemons
);
```

### PL/SQL functions:

```
create or replace function insert_pokemon(
    p_name varchar2,
    p_generation number
) return number is
    v_id number;
begin
    select max(id) into v_id from p_pokemons;

    if v_id is null then v_id := 0;
    end if;

    v_id := v_id + 1;

    insert into p_pokemons values (v_id, p_name, p_generation, null);
    return v_id;
end;
```

```
create or replace function insert_pokemon_image(
    p_image in blob,
    p_pokemon_id in number
) return number is
    v_id number;
    v_image ordimage;
    v_image_signature ordimagesignature;
begin   
    select max(id) into v_id from p_pokemons_images;

    if v_id is null then v_id := 0;
    end if;

    v_id := v_id + 1;

    v_image := ordimage(p_image, 1);

    insert into p_pokemons_images values (v_id, p_pokemon_id, v_image, ordimagesignature.init());

    select image_signature into v_image_signature from p_pokemons_images where id = v_id for update;
    v_image_signature.generateSignature(v_image);

    update p_pokemons_images set image_signature = v_image_signature where id = v_id;

    return v_id;
end;
```

```
create or replace function insert_pokemon_sound (
    p_sound in blob,
    p_pokemon_id in number
) return number is
    v_id number;
begin
    select max(id) into v_id from p_pokemons_sounds;

    if v_id is null then v_id := 0;
    end if;

    v_id := v_id + 1;

    insert into p_pokemons_sounds values(v_id, p_pokemon_id, ordaudio(p_sound, 1));
    return v_id;
end;
```

```
create or replace function get_pokemon_image (
    p_image_id in number
) return blob is
    v_image ordimage;
begin
    select image into v_image from p_pokemons_images where id = p_image_id;

    return v_image.getContent();
end;
```

```
create or replace function get_pokemon_sound (
    p_sound_id in number
) return blob is
    v_sound ordaudio;
begin
    select sound into v_sound from p_pokemons_sounds where id = p_sound_id;

    return v_sound.getContent();
end;
```

```
create or replace function get_similar_pokemons_ids (
    p_image in blob
) return varchar2 is
    v_image ordimage;
    v_image_signature ordimagesignature;
    v_id number;
    v_pokemons_ids varchar2(255);
begin
    v_image := ordimage(p_image, 1);
    v_image.setProperties;

    v_image_signature := ordimagesignature.init();
    dbms_lob.createtemporary(v_image_signature.signature, true);
    v_image_signature.generateSignature(v_image);

    for x in (
        select *
        from p_pokemons_images pi
        where ORDImageSignature.evaluateScore(
                                    v_image_signature, 
                                    pi.image_signature,
                                    'color=1,texture=0.3,shape=0.5,location=0.3') < 25
    ) loop
        select pi.pokemon_id
        into v_id
        from p_pokemons_images pi
        where x.id = pi.id;

        if length(v_pokemons_ids) is null then
            v_pokemons_ids := v_id;
        else 
            v_pokemons_ids := v_pokemons_ids || ',' || v_id;
        end if;
    end loop;

    return v_pokemons_ids;
end;
```
