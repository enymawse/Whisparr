import React from 'react';
import { ParseModel } from 'App/State/ParseAppState';
import FieldSet from 'Components/FieldSet';
import MovieFormats from 'Movie/MovieFormats';
import MovieTitleLink from 'Movie/MovieTitleLink';
import translate from 'Utilities/String/translate';
import ParseResultItem from './ParseResultItem';
import styles from './ParseResult.css';

interface ParseResultProps {
  item: ParseModel;
}

function ParseResult(props: ParseResultProps) {
  const { item } = props;
  const {
    customFormats,
    customFormatScore,
    languages,
    parsedMovieInfo,
    movie,
  } = item;

  const {
    releaseTitle,
    isScene,
    movieTitle,
    movieTitles,
    studioTitle,
    releaseDate,
    year,
    edition,
    releaseGroup,
    releaseHash,
    quality,
    tmdbId,
    imdbId,
  } = parsedMovieInfo;

  const finalLanguages = languages ?? parsedMovieInfo.languages;

  return (
    <div>
      <FieldSet legend={translate('Release')}>
        <ParseResultItem
          title={translate('ReleaseTitle')}
          data={releaseTitle}
        />

        <ParseResultItem
          title={translate('ReleaseType')}
          data={isScene ? 'Scene' : 'Movie'}
        />

        {isScene ? (
          <ParseResultItem
            title={translate('StudioTitle')}
            data={studioTitle}
          />
        ) : (
          <ParseResultItem title={translate('MovieTitle')} data={movieTitle} />
        )}

        {isScene ? (
          <ParseResultItem
            title={translate('ReleaseDate')}
            data={releaseDate}
          />
        ) : (
          <ParseResultItem
            title={translate('Year')}
            data={year > 0 ? year : '-'}
          />
        )}

        <ParseResultItem
          title={translate('Edition')}
          data={edition ? edition : '-'}
        />

        <ParseResultItem
          title={translate('AllTitles')}
          data={movieTitles?.length > 0 ? movieTitles.join(', ') : '-'}
        />

        <ParseResultItem
          title={translate('ReleaseGroup')}
          data={releaseGroup ?? '-'}
        />

        <ParseResultItem
          title={translate('ReleaseHash')}
          data={releaseHash ? releaseHash : '-'}
        />

        {tmdbId ? (
          <ParseResultItem title={translate('TMDBId')} data={tmdbId} />
        ) : null}

        {imdbId ? (
          <ParseResultItem title={translate('IMDbId')} data={imdbId} />
        ) : null}
      </FieldSet>

      <FieldSet legend={translate('Quality')}>
        <div className={styles.container}>
          <div className={styles.column}>
            <ParseResultItem
              title={translate('Quality')}
              data={quality.quality.name}
            />
            <ParseResultItem
              title={translate('Proper')}
              data={
                quality.revision.version > 1 && !quality.revision.isRepack
                  ? 'True'
                  : '-'
              }
            />

            <ParseResultItem
              title={translate('Repack')}
              data={quality.revision.isRepack ? translate('True') : '-'}
            />
          </div>

          <div className={styles.column}>
            <ParseResultItem
              title={translate('Version')}
              data={
                quality.revision.version > 1 ? quality.revision.version : '-'
              }
            />

            <ParseResultItem
              title={translate('Real')}
              data={quality.revision.real ? translate('True') : '-'}
            />
          </div>
        </div>
      </FieldSet>

      <FieldSet legend={translate('Languages')}>
        <ParseResultItem
          title={translate('Languages')}
          data={finalLanguages.map((l) => l.name).join(', ')}
        />
      </FieldSet>

      <FieldSet legend={translate('Details')}>
        <ParseResultItem
          title={translate('MatchedToMovie')}
          data={
            movie ? (
              <MovieTitleLink
                foreignId={movie.foreignId}
                title={movie.title}
                year={movie.year}
              />
            ) : (
              '-'
            )
          }
        />

        {movie && movie.originalLanguage ? (
          <ParseResultItem
            title={translate('OriginalLanguage')}
            data={movie.originalLanguage.name}
          />
        ) : null}

        <ParseResultItem
          title={translate('CustomFormats')}
          data={
            customFormats?.length ? (
              <MovieFormats formats={customFormats} />
            ) : (
              '-'
            )
          }
        />

        <ParseResultItem
          title={translate('CustomFormatScore')}
          data={customFormatScore}
        />
      </FieldSet>
    </div>
  );
}

export default ParseResult;
